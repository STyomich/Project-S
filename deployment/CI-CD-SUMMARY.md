# CI/CD Deployment Summary - Project-S

This document summarizes all CI/CD components created for Project-S Azure deployment.

## 📋 Files Created

### 1. **GitHub Actions Workflow**
**File:** `.github/workflows/ci-cd-azure.yml`

**What it does:**
- Automatically runs on push to `main` (prod) and `dev` (dev) branches
- Runs on pull requests (tests only, no deployment)
- Executes in stages: Test → Build → Deploy → Cleanup

**Stages:**
| Stage | Trigger | Action |
|-------|---------|--------|
| **Test** | All branches | Run backend/frontend tests |
| **Build Images** | Push to main/dev | Build Docker images, push to ACR |
| **Deploy Dev** | Push to dev | Deploy to Azure Container Instances |
| **Deploy Prod** | Push to main | Deploy to Azure Kubernetes Service (AKS) |
| **Cleanup** | After deployment | Delete old images from registry |

### 2. **Deployment Setup Guide**
**File:** `deployment/CI-CD-SETUP.md`

**Contains:**
- Step-by-step GitHub Secrets configuration
- Azure resource setup commands
- Kubernetes deployment manifests examples
- Troubleshooting guide
- Best practices for CI/CD

**⚠️ MUST READ** before deploying - explains all required configuration

### 3. **Kubernetes Manifests** (in `k8s/` directory)

| File | Purpose |
|------|---------|
| `00-namespace-and-secrets.yaml` | Namespaces, ConfigMap, Secrets, RBAC, network policies |
| `01-gateway-deployment.yaml` | API Gateway (Ocelot) deployment, service, auto-scaling |
| `02-users-service-deployment.yaml` | UsersService deployment with PostgreSQL/Redis/RabbitMQ |
| `03-notifications-service-deployment.yaml` | NotificationsService deployment with MySQL/RabbitMQ |
| `04-utilities-service-deployment.yaml` | UtilitiesService deployment with MongoDB/RabbitMQ |
| `05-ingress-and-policies.yaml` | Ingress routing, certificates, network policies, resource quotas |

## 🚀 Quick Start Checklist

### Phase 1: GitHub Configuration (5 mins)
- [ ] Go to **Settings → Secrets and variables → Actions**
- [ ] Add all required secrets (see CI-CD-SETUP.md)
- [ ] Create Azure Service Principal
- [ ] Get ACR credentials
- [ ] Get AKS details

### Phase 2: Azure Resources (15 mins)
- [ ] Create Azure Container Registry (ACR)
- [ ] Create Azure Kubernetes Service (AKS) cluster
- [ ] Create databases (PostgreSQL, MySQL, MongoDB)
- [ ] Create Redis cache
- [ ] Create RabbitMQ instance (e.g., Azure Service Bus)

### Phase 3: Kubernetes Setup (10 mins)
- [ ] Get AKS credentials: `az aks get-credentials --resource-group ... --name ...`
- [ ] Apply Kubernetes manifests: `kubectl apply -f k8s/`
- [ ] Verify namespaces: `kubectl get ns`
- [ ] Verify secrets: `kubectl get secrets -n project-s-prod`

### Phase 4: GitHub Workflow (5 mins)
- [ ] Push code to `dev` branch
- [ ] Check GitHub Actions → Workflows
- [ ] Verify tests pass
- [ ] Verify Docker images built
- [ ] Verify deployment to dev environment

### Phase 5: Production Deployment (5 mins)
- [ ] Merge to `main` branch
- [ ] Watch GitHub Actions for production deployment
- [ ] Verify in Azure portal

## 📊 Workflow Execution Flow

```
┌─────────────────────────────────────────────────────────────┐
│ Code Push to GitHub (main/dev)                              │
└────────────────────┬────────────────────────────────────────┘
                     │
         ┌───────────▼───────────┐
         │   Backend Tests       │
         │  (xUnit - .NET)       │
         └───────────┬───────────┘
                     │
         ┌───────────▼───────────┐
         │   Frontend Tests      │
         │  (Vitest - Angular)   │
         └───────────┬───────────┘
                     │
    ┌────────────────▼────────────────┐
    │ Tests Pass? Push to Dev/Main?   │
    └────────────┬────────────────────┘
                 │
        ┌────────▼────────┐
        │  Build & Push   │
        │  Docker Images  │
        │   to ACR        │
        └────────┬────────┘
                 │
         ┌───────▼───────┐
    ┌────┤ Branch Check?  ├────┐
    │    └────────────────┘    │
    │                           │
┌───▼──────────┐         ┌──────▼──────┐
│ dev branch   │         │ main branch │
│     ↓        │         │      ↓      │
│ Deploy to    │         │ Deploy to   │
│ Container    │         │ Kubernetes  │
│ Instances    │         │ (AKS)       │
└──────────────┘         └─────────────┘
```

## 🔐 Environment Variables Management

### Development (.env from GitHub Secrets)
```bash
# Created in deployment/.env.backend
POSTGRES_HOST=dev-postgres.database.azure.com
MYSQL_HOST=dev-mysql.database.azure.com
MONGODB_CONNECTION_STRING=mongodb://...
REDIS_HOST=dev-redis.redis.cache.windows.net
RABBITMQ_HOST=dev-rabbitmq.servicebus.windows.net
ASPNETCORE_ENVIRONMENT=Development
```

### Production (Kubernetes Secrets)
```bash
# Applied via kubectl from k8s/00-namespace-and-secrets.yaml
# Stored in: postgres-credentials, mysql-credentials, mongodb-credentials
# Access via: valueFrom.secretKeyRef in pod specs
ASPNETCORE_ENVIRONMENT=Production
```

## 🐳 Docker Images Built

| Image | Registry Path |
|-------|---------------|
| Gateway | `projectsregistry.azurecr.io/project-s-gateway:sha` |
| UsersService | `projectsregistry.azurecr.io/project-s-users-service:sha` |
| NotificationsService | `projectsregistry.azurecr.io/project-s-notifications-service:sha` |
| UtilitiesService | `projectsregistry.azurecr.io/project-s-utilities-service:sha` |
| Frontend | `projectsregistry.azurecr.io/project-s-frontend:sha` |

**Image Tagging:**
- `project-s-gateway:abc123de` (commit SHA)
- `project-s-gateway:latest-dev` (branch latest)

## 🔍 Monitoring & Debugging

### GitHub Actions Logs
```
Settings → Actions → Select run → View detailed logs
```

### Kubernetes Troubleshooting
```bash
# View pod status
kubectl get pods -n project-s-prod

# View pod logs
kubectl logs <pod-name> -n project-s-prod --tail=50

# Describe pod (events, conditions)
kubectl describe pod <pod-name> -n project-s-prod

# Check service connectivity
kubectl port-forward svc/project-s-gateway 5000:80 -n project-s-prod

# Scale deployment
kubectl scale deployment project-s-gateway --replicas=5 -n project-s-prod
```

### Azure Portal Monitoring
1. **Container Registry** → Repositories → View images
2. **Kubernetes Service** → Workloads → View pods/deployments
3. **Monitor** → Check pod resource utilization
4. **Alerts** → Setup failure notifications

## 🛠️ Manual Overrides

### Deploy Specific Image
```bash
kubectl set image deployment/project-s-gateway \
  gateway=projectsregistry.azurecr.io/project-s-gateway:specific-sha \
  -n project-s-prod

kubectl rollout status deployment/project-s-gateway -n project-s-prod
```

### Rollback Deployment
```bash
kubectl rollout history deployment/project-s-gateway -n project-s-prod
kubectl rollout undo deployment/project-s-gateway -n project-s-prod
```

### Manually Trigger Workflow
1. GitHub → Actions → "CI/CD - Build, Test & Deploy to Azure"
2. Click "Run workflow"
3. Select branch (main/dev)
4. Click "Run"

## ✅ Post-Deployment Checklist

After deployment completes:

- [ ] Check GitHub Actions logs for success ✅
- [ ] Verify all pods running: `kubectl get pods -n project-s-prod`
- [ ] Test Gateway endpoint: `curl https://project-s.azurewebsites.net/api/health`
- [ ] Check pod resource usage: `kubectl top pods -n project-s-prod`
- [ ] Verify services can communicate
- [ ] Test critical user flows (login, register, etc.)
- [ ] Monitor logs for errors: `kubectl logs -n project-s-prod -l app=gateway --tail=100`
- [ ] Check Azure Portal for cost/resource utilization

## 📞 Troubleshooting Guide

### Tests Fail in GitHub Actions
1. Check test output in workflow logs
2. Run tests locally: `dotnet test` or `npm test`
3. Verify dependencies installed: `dotnet restore` / `npm install`
4. Check database connectivity (for integration tests)

### Docker Image Not Found
1. Verify ACR credentials in secrets
2. Check image pushed: `az acr repository list --name projectsregistry`
3. Verify Dockerfile exists at service root

### Pods Crash After Deployment
1. Check pod logs: `kubectl logs <pod-name> -n project-s-prod`
2. Verify environment variables: `kubectl set env ... --list`
3. Check resource limits not exceeded: `kubectl top pods`
4. Verify secrets mounted: `kubectl describe pod <pod-name>`

### Database Connection Errors
1. Verify connection string in secrets
2. Check network policies allow traffic
3. Verify database exists and is accessible
4. Test connection from pod: `kubectl exec -it <pod-name> /bin/bash`

## 📚 Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Azure Kubernetes Service](https://docs.microsoft.com/en-us/azure/aks/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [Azure Container Registry](https://docs.microsoft.com/en-us/azure/container-registry/)
- [Project-S AGENTS.md](../AGENTS.md) - Architecture & conventions

---

**Created:** April 18, 2026  
**Project:** Project-S Microservices  
**Status:** Ready for deployment ✅
