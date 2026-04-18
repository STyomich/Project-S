#!/bin/bash

# Project-S CI/CD Setup Script
# This script helps configure GitHub Secrets for Project-S deployment to Azure

set -e

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}╔════════════════════════════════════════════════════════════╗${NC}"
echo -e "${BLUE}║     Project-S CI/CD Setup - GitHub Secrets Configuration    ║${NC}"
echo -e "${BLUE}╚════════════════════════════════════════════════════════════╝${NC}"
echo ""

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Check prerequisites
echo -e "${YELLOW}Checking prerequisites...${NC}"

if ! command_exists az; then
    echo -e "${RED}❌ Azure CLI not found. Install from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Azure CLI${NC}"

if ! command_exists git; then
    echo -e "${RED}❌ Git not found${NC}"
    exit 1
fi
echo -e "${GREEN}✓ Git${NC}"

if ! command_exists jq; then
    echo -e "${YELLOW}⚠ jq not found (optional but recommended)${NC}"
fi

echo ""
echo -e "${YELLOW}Step 1: Azure Login${NC}"
echo "Logging into Azure..."
az login
echo -e "${GREEN}✓ Logged in${NC}"

echo ""
echo -e "${YELLOW}Step 2: Get/Create Resource Group${NC}"
read -p "Enter Azure Resource Group name: " RESOURCE_GROUP
read -p "Enter Azure Region (e.g., eastus): " REGION

# Check if resource group exists
if az group exists --name "$RESOURCE_GROUP" --query value -o tsv | grep -q "true"; then
    echo -e "${GREEN}✓ Resource group '$RESOURCE_GROUP' exists${NC}"
else
    echo "Creating resource group..."
    az group create --name "$RESOURCE_GROUP" --location "$REGION"
    echo -e "${GREEN}✓ Resource group created${NC}"
fi

SUBSCRIPTION_ID=$(az account show --query id -o tsv)
echo -e "${GREEN}Subscription ID: $SUBSCRIPTION_ID${NC}"

echo ""
echo -e "${YELLOW}Step 3: Create Azure Service Principal${NC}"
echo "This is required for GitHub Actions to authenticate with Azure"

SP_NAME="github-actions-project-s"
read -p "Enter Service Principal name [$SP_NAME]: " SP_NAME_INPUT
SP_NAME="${SP_NAME_INPUT:-$SP_NAME}"

echo "Creating Service Principal: $SP_NAME"
SP_JSON=$(az ad sp create-for-rbac \
    --name "$SP_NAME" \
    --role Contributor \
    --scopes "/subscriptions/$SUBSCRIPTION_ID/resourceGroups/$RESOURCE_GROUP" \
    --json-auth)

echo -e "${GREEN}✓ Service Principal created${NC}"
echo ""
echo -e "${BLUE}>>> ADD THIS AS GITHUB SECRET 'AZURE_CREDENTIALS' <<<${NC}"
echo "$SP_JSON"
echo ""

echo -e "${YELLOW}Step 4: Create/Get Azure Container Registry${NC}"

read -p "Enter ACR name (e.g., projectsregistry): " ACR_NAME

# Check if ACR exists
if az acr show --resource-group "$RESOURCE_GROUP" --name "$ACR_NAME" &>/dev/null; then
    echo -e "${GREEN}✓ ACR '$ACR_NAME' exists${NC}"
else
    echo "Creating ACR: $ACR_NAME"
    az acr create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$ACR_NAME" \
        --sku Basic
    echo -e "${GREEN}✓ ACR created${NC}"
fi

# Get ACR credentials
ACR_CREDS=$(az acr credential show --resource-group "$RESOURCE_GROUP" --name "$ACR_NAME")
ACR_USERNAME=$(echo "$ACR_CREDS" | jq -r '.username')
ACR_PASSWORD=$(echo "$ACR_CREDS" | jq -r '.passwords[0].value')

echo ""
echo -e "${BLUE}>>> ADD THESE AS GITHUB SECRETS <<<${NC}"
echo "AZURE_REGISTRY_NAME: $ACR_NAME"
echo "AZURE_REGISTRY_USERNAME: $ACR_USERNAME"
echo "AZURE_REGISTRY_PASSWORD: $ACR_PASSWORD"
echo ""

echo -e "${YELLOW}Step 5: Create/Get Azure Kubernetes Service${NC}"

read -p "Enter AKS cluster name (e.g., project-s-aks): " AKS_NAME
read -p "Enter number of nodes [3]: " NODE_COUNT
NODE_COUNT="${NODE_COUNT:-3}"

# Check if AKS exists
if az aks show --resource-group "$RESOURCE_GROUP" --name "$AKS_NAME" &>/dev/null; then
    echo -e "${GREEN}✓ AKS cluster '$AKS_NAME' exists${NC}"
else
    echo "Creating AKS cluster: $AKS_NAME (this may take 10+ minutes)"
    az aks create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$AKS_NAME" \
        --node-count "$NODE_COUNT" \
        --enable-managed-identity \
        --generate-ssh-keys
    echo -e "${GREEN}✓ AKS cluster created${NC}"
fi

# Attach ACR to AKS
echo "Attaching ACR to AKS..."
az aks update \
    --resource-group "$RESOURCE_GROUP" \
    --name "$AKS_NAME" \
    --attach-acr "$ACR_NAME"
echo -e "${GREEN}✓ ACR attached${NC}"

echo ""
echo -e "${BLUE}>>> ADD THIS AS GITHUB SECRET <<<${NC}"
echo "AZURE_RESOURCE_GROUP: $RESOURCE_GROUP"
echo "AKS_CLUSTER_NAME: $AKS_NAME"
echo ""

echo -e "${YELLOW}Step 6: Database and Infrastructure Configuration${NC}"

echo ""
echo -e "${BLUE}POSTGRESQL (UsersService):${NC}"
read -p "Enter PostgreSQL server name: " POSTGRES_HOST
read -p "Enter PostgreSQL admin username: " POSTGRES_USER
read -s -p "Enter PostgreSQL admin password: " POSTGRES_PASSWORD
echo ""
read -p "Enter PostgreSQL database name [usersdb]: " POSTGRES_DATABASE
POSTGRES_DATABASE="${POSTGRES_DATABASE:-usersdb}"
POSTGRES_PORT="5432"

echo ""
echo -e "${BLUE}MYSQL (NotificationsService):${NC}"
read -p "Enter MySQL server name: " MYSQL_HOST
read -p "Enter MySQL admin username: " MYSQL_USER
read -s -p "Enter MySQL admin password: " MYSQL_PASSWORD
echo ""
read -p "Enter MySQL database name [notificationsdb]: " MYSQL_DATABASE
MYSQL_DATABASE="${MYSQL_DATABASE:-notificationsdb}"
MYSQL_PORT="3306"

echo ""
echo -e "${BLUE}MONGODB (UtilitiesService):${NC}"
read -p "Enter MongoDB connection string: " MONGODB_CONNECTION_STRING
read -p "Enter MongoDB database name [Utilities]: " MONGODB_DATABASE
MONGODB_DATABASE="${MONGODB_DATABASE:-Utilities}"

echo ""
echo -e "${BLUE}REDIS (Caching):${NC}"
read -p "Enter Redis server name: " REDIS_HOST
read -p "Enter Redis port [6379]: " REDIS_PORT
REDIS_PORT="${REDIS_PORT:-6379}"

echo ""
echo -e "${BLUE}RABBITMQ (Messaging):${NC}"
read -p "Enter RabbitMQ server name: " RABBITMQ_HOST
read -p "Enter RabbitMQ port [5672]: " RABBITMQ_PORT
RABBITMQ_PORT="${RABBITMQ_PORT:-5672}"
read -p "Enter RabbitMQ username [guest]: " RABBITMQ_USERNAME
RABBITMQ_USERNAME="${RABBITMQ_USERNAME:-guest}"
read -s -p "Enter RabbitMQ password: " RABBITMQ_PASSWORD
echo ""

echo ""
echo -e "${YELLOW}Step 7: Authentication & API Configuration${NC}"

read -s -p "Enter JWT Token Key (minimum 32 characters): " TOKEN_KEY
echo ""
read -p "Enter JWT Issuer (e.g., https://project-s.azurewebsites.net): " JWT_ISSUER
read -p "Enter JWT Audience (e.g., project-s-api): " JWT_AUDIENCE
read -p "Enter API URL for frontend (e.g., https://project-s-api.azurewebsites.net/api): " API_URL

echo ""
echo -e "${YELLOW}Step 8: Generate GitHub Secrets Summary${NC}"

echo ""
echo -e "${GREEN}╔═══════════════════════════════════════════════════════════╗${NC}"
echo -e "${GREEN}║           GITHUB SECRETS TO ADD                            ║${NC}"
echo -e "${GREEN}╚═══════════════════════════════════════════════════════════╝${NC}"
echo ""
echo "Go to: GitHub → Repository → Settings → Secrets and variables → Actions"
echo ""

cat > /tmp/github-secrets.sh << EOF
# Copy & paste these secrets one by one in GitHub

AZURE_CREDENTIALS='$SP_JSON'

AZURE_REGISTRY_NAME='$ACR_NAME'
AZURE_REGISTRY_USERNAME='$ACR_USERNAME'
AZURE_REGISTRY_PASSWORD='$ACR_PASSWORD'

AZURE_RESOURCE_GROUP='$RESOURCE_GROUP'
AKS_CLUSTER_NAME='$AKS_NAME'

POSTGRES_HOST='$POSTGRES_HOST'
POSTGRES_PORT='$POSTGRES_PORT'
POSTGRES_DATABASE='$POSTGRES_DATABASE'
POSTGRES_USER='$POSTGRES_USER'
POSTGRES_PASSWORD='$POSTGRES_PASSWORD'

MYSQL_HOST='$MYSQL_HOST'
MYSQL_PORT='$MYSQL_PORT'
MYSQL_DATABASE='$MYSQL_DATABASE'
MYSQL_USER='$MYSQL_USER'
MYSQL_PASSWORD='$MYSQL_PASSWORD'

MONGODB_CONNECTION_STRING='$MONGODB_CONNECTION_STRING'
MONGODB_DATABASE='$MONGODB_DATABASE'

REDIS_HOST='$REDIS_HOST'
REDIS_PORT='$REDIS_PORT'

RABBITMQ_HOST='$RABBITMQ_HOST'
RABBITMQ_PORT='$RABBITMQ_PORT'
RABBITMQ_USERNAME='$RABBITMQ_USERNAME'
RABBITMQ_PASSWORD='$RABBITMQ_PASSWORD'
RABBITMQ_USERS_EXCHANGE='user.exchange'

TOKEN_KEY='$TOKEN_KEY'
JWT_ISSUER='$JWT_ISSUER'
JWT_AUDIENCE='$JWT_AUDIENCE'

API_URL='$API_URL'
EOF

cat /tmp/github-secrets.sh

echo ""
echo -e "${GREEN}✓ Secrets file saved to: /tmp/github-secrets.sh${NC}"
echo ""

echo -e "${YELLOW}Step 9: Setup Kubernetes Resources${NC}"

echo "Getting AKS credentials..."
az aks get-credentials \
    --resource-group "$RESOURCE_GROUP" \
    --name "$AKS_NAME" \
    --overwrite-existing
echo -e "${GREEN}✓ AKS credentials configured${NC}"

echo ""
echo "Creating Kubernetes namespaces..."
kubectl create namespace project-s-prod --dry-run=client -o yaml | kubectl apply -f -
kubectl create namespace project-s-dev --dry-run=client -o yaml | kubectl apply -f -
echo -e "${GREEN}✓ Namespaces created${NC}"

echo ""
echo -e "${YELLOW}Next Steps:${NC}"
echo "1. Add all GitHub Secrets from the list above"
echo "2. Push code to 'dev' branch to trigger workflow"
echo "3. Monitor GitHub Actions for build/deploy status"
echo "4. Check pods: kubectl get pods -n project-s-prod"
echo "5. View logs: kubectl logs -n project-s-prod -l app=gateway"
echo ""

echo -e "${GREEN}✓ Setup Complete!${NC}"
echo ""
echo "For more details, see: deployment/CI-CD-SETUP.md"
