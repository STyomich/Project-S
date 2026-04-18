# CI/CD Deployment Guide - GitHub Actions to Azure

This guide explains the CI/CD workflow for Project-S and how to configure it for deployment to Azure.

## Workflow Overview

The `ci-cd-azure.yml` workflow automates:

1. **Testing Stage** (runs on all branches)
   - Backend unit tests (.NET xUnit) for all microservices
   - Frontend unit tests (Angular/Vitest)
   - Build validation

2. **Build Stage** (runs on push to main/dev)
   - Builds Docker images for all services
   - Pushes images to Azure Container Registry (ACR)

3. **Deploy Stage** (conditional based on branch)
   - **Dev**: Deploys to Azure Container Instances
   - **Prod**: Deploys to Azure Kubernetes Service (AKS)

4. **Cleanup Stage**
   - Removes untagged images older than 7 days from ACR

## Prerequisites

### Azure Resources Required

1. **Azure Container Registry (ACR)**
   - Used to store Docker images
   - Name format: `yourregistryname` (e.g., `projectsregistry`)

2. **Dev Environment**
   - Azure Container Instances (ACI) for lightweight deployment
   - Resource Group for containers

3. **Production Environment**
   - Azure Kubernetes Service (AKS) cluster
   - Kubernetes namespaces: `project-s-prod`, `project-s-dev`
   - Databases (PostgreSQL, MySQL, MongoDB)
   - Redis cache
   - RabbitMQ message broker

### GitHub Configuration

#### Step 1: Create GitHub Secrets

Go to: **Settings → Secrets and variables → Actions → New repository secret**

Add the following secrets:

**Azure Infrastructure Secrets:**
```
AZURE_CREDENTIALS            # Azure Service Principal credentials (JSON format)
AZURE_REGISTRY_NAME          # ACR name (without .azurecr.io)
AZURE_REGISTRY_USERNAME      # ACR username
AZURE_REGISTRY_PASSWORD      # ACR password
AZURE_RESOURCE_GROUP         # Azure Resource Group name
AKS_CLUSTER_NAME             # AKS cluster name for production
```

**Database Secrets:**
```
POSTGRES_HOST                # PostgreSQL server address
POSTGRES_PORT                # PostgreSQL port (default: 5432)
POSTGRES_DATABASE            # PostgreSQL database name (usersdb)
POSTGRES_USER                # PostgreSQL username
POSTGRES_PASSWORD            # PostgreSQL password

MYSQL_HOST                   # MySQL server address
MYSQL_PORT                   # MySQL port (default: 3306)
MYSQL_DATABASE               # MySQL database name (notificationsdb)
MYSQL_USER                   # MySQL username
MYSQL_PASSWORD               # MySQL password

MONGODB_CONNECTION_STRING    # MongoDB connection string
MONGODB_DATABASE             # MongoDB database name (Utilities)
```

**Cache & Message Bus Secrets:**
```
REDIS_HOST                   # Redis server address
REDIS_PORT                   # Redis port (default: 6379)

RABBITMQ_HOST                # RabbitMQ server address
RABBITMQ_PORT                # RabbitMQ port (default: 5672)
RABBITMQ_USERNAME            # RabbitMQ username (default: guest)
RABBITMQ_PASSWORD            # RabbitMQ password
RABBITMQ_USERS_EXCHANGE      # RabbitMQ exchange name (user.exchange)
```

**Authentication Secrets:**
```
TOKEN_KEY                    # JWT signing key (32+ characters)
JWT_ISSUER                   # JWT issuer URL
JWT_AUDIENCE                 # JWT audience
```

**Frontend Secrets:**
```
API_URL                      # Backend API URL (https://your-gateway.azurewebsites.net/api)
```

**Azure Static Web Apps Tokens:**
```
AZURE_STATIC_WEB_APPS_TOKEN_DEV    # Token for dev environment
AZURE_STATIC_WEB_APPS_TOKEN_PROD   # Token for production
```

#### Step 2: Create Azure Service Principal

Run this command to create credentials for `AZURE_CREDENTIALS`:

```bash
az ad sp create-for-rbac \
  --name "github-actions-project-s" \
  --role Contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group}
```

Output format (copy as JSON to `AZURE_CREDENTIALS`):
```json
{
  "clientId": "...",
  "clientSecret": "...",
  "subscriptionId": "...",
  "tenantId": "..."
}
```

#### Step 3: Configure Azure Container Registry

```bash
# Create ACR
az acr create \
  --resource-group {resource-group} \
  --name projectsregistry \
  --sku Basic

# Get credentials
az acr credential show \
  --resource-group {resource-group} \
  --name projectsregistry
```

Use the credentials for `AZURE_REGISTRY_USERNAME` and `AZURE_REGISTRY_PASSWORD` secrets.

#### Step 4: Setup Kubernetes for Production (AKS)

```bash
# Create AKS cluster
az aks create \
  --resource-group {resource-group} \
  --name project-s-aks \
  --node-count 3 \
  --enable-managed-identity

# Create namespaces
kubectl create namespace project-s-prod
kubectl create namespace project-s-dev

# Attach ACR to AKS
az aks update \
  --name project-s-aks \
  --resource-group {resource-group} \
  --attach-acr projectsregistry
```

#### Step 5: Create Kubernetes Deployment Manifests

Create `k8s/` directory with the following files:

**k8s/gateway-deployment.yaml**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: project-s-gateway
  namespace: project-s-prod
spec:
  replicas: 2
  selector:
    matchLabels:
      app: gateway
  template:
    metadata:
      labels:
        app: gateway
    spec:
      containers:
      - name: gateway
        image: projectsregistry.azurecr.io/project-s-gateway:latest
        ports:
        - containerPort: 5000
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: TokenKey
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: token-key
        - name: USERS_API_URL
          value: "http://project-s-users-service:8080"
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /api/health
            port: 5000
          initialDelaySeconds: 30
          periodSeconds: 10
        readinessProbe:
          httpGet:
            path: /api/health
            port: 5000
          initialDelaySeconds: 10
          periodSeconds: 5
---
apiVersion: v1
kind: Service
metadata:
  name: project-s-gateway
  namespace: project-s-prod
spec:
  type: LoadBalancer
  selector:
    app: gateway
  ports:
  - port: 5000
    targetPort: 5000
```

**k8s/users-service-deployment.yaml**
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: project-s-users-service
  namespace: project-s-prod
spec:
  replicas: 2
  selector:
    matchLabels:
      app: users-service
  template:
    metadata:
      labels:
        app: users-service
    spec:
      containers:
      - name: users-service
        image: projectsregistry.azurecr.io/project-s-users-service:latest
        ports:
        - containerPort: 8080
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
        - name: POSTGRES_HOST
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: postgres-host
        - name: POSTGRES_PASSWORD
          valueFrom:
            secretKeyRef:
              name: app-secrets
              key: postgres-password
        resources:
          requests:
            memory: "512Mi"
            cpu: "250m"
          limits:
            memory: "1Gi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 8080
          initialDelaySeconds: 30
          periodSeconds: 10
---
apiVersion: v1
kind: Service
metadata:
  name: project-s-users-service
  namespace: project-s-prod
spec:
  selector:
    app: users-service
  ports:
  - port: 8080
    targetPort: 8080
```

Create similar manifests for `notifications-service` and `utilities-service`.

#### Step 6: Create Kubernetes Secrets

```bash
# Create secret for application config
kubectl create secret generic app-secrets \
  --from-literal=token-key='your-token-key' \
  --from-literal=postgres-host='your-postgres-host' \
  --from-literal=postgres-password='your-password' \
  --from-literal=mysql-password='your-password' \
  --from-literal=rabbitmq-password='your-password' \
  -n project-s-prod

# Create image pull secret for ACR
kubectl create secret docker-registry acr-secret \
  --docker-server=projectsregistry.azurecr.io \
  --docker-username={username} \
  --docker-password={password} \
  -n project-s-prod
```

## Workflow Triggers

| Branch | Event | Action |
|--------|-------|--------|
| `main` | Push | Run tests → Build images → Deploy to **Production (AKS)** |
| `dev` | Push | Run tests → Build images → Deploy to **Dev (ACI)** |
| `*` | Pull Request | Run tests only (no deployment) |

## Monitoring Deployment

### View GitHub Actions logs:
```
Settings → Actions → Select workflow run → View detailed logs
```

### Monitor Azure deployments:

**For ACI (Dev):**
```bash
az container show \
  --resource-group {resource-group} \
  --name project-s-gateway-dev \
  --query "containers[0].instanceView.events" -o table
```

**For AKS (Prod):**
```bash
# View pod status
kubectl get pods -n project-s-prod

# View logs
kubectl logs -n project-s-prod deployment/project-s-gateway --tail=50

# View deployment events
kubectl describe deployment project-s-gateway -n project-s-prod
```

## Troubleshooting

### Common Issues

1. **Image Pull Failed**
   - Verify ACR credentials in secrets
   - Check image exists: `az acr repository show --name projectsregistry --image project-s-gateway:latest`

2. **Pod Crash Loop**
   - Check environment variables are set correctly
   - View logs: `kubectl logs <pod-name> -n project-s-prod`
   - Verify database connectivity

3. **Deployment Rollback**
   ```bash
   kubectl rollout history deployment/project-s-gateway -n project-s-prod
   kubectl rollout undo deployment/project-s-gateway -n project-s-prod
   ```

4. **Tests Failing in Pipeline**
   - Check test output in GitHub Actions logs
   - Run locally: `dotnet test` or `npm test`
   - Verify all dependencies are installed

## Best Practices

✅ **Always run tests before deployment** - The workflow enforces this
✅ **Use environment secrets** - Never commit sensitive data
✅ **Tag releases** - Creates deployment artifacts
✅ **Monitor deployments** - Check logs immediately after deployment
✅ **Implement health checks** - Liveness/readiness probes prevent stuck pods
✅ **Use image tags** - Production uses specific commit SHA, not latest
✅ **Cleanup old images** - Prevents ACR storage bloat

## Manual Deployment (if needed)

To manually trigger deployment without code changes:

1. Go to Actions tab
2. Select "CI/CD - Build, Test & Deploy to Azure"
3. Click "Run workflow"
4. Select branch (main for prod, dev for dev)
5. Click green "Run workflow" button

## Environment-Specific Configuration

### Development (.env.backend for dev)
- Connection strings point to dev databases
- RabbitMQ on dev infrastructure
- Redis cache on dev infrastructure
- Logging level: Debug

### Production (.env.backend for prod)
- Connection strings point to prod databases
- RabbitMQ on prod infrastructure
- Redis cache on prod infrastructure (replicated)
- Logging level: Warning
- SSL/TLS certificates enabled

## Next Steps

1. ✅ Add all required secrets to GitHub
2. ✅ Create Azure Service Principal
3. ✅ Setup ACR and AKS
4. ✅ Create Kubernetes manifests
5. ✅ Push code to trigger workflow
6. ✅ Monitor first deployment in GitHub Actions
7. ✅ Verify services in Azure Portal

For questions, refer to [AGENTS.md](../AGENTS.md) for architecture details.
