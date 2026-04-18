# Deployment Documentation

This directory contains all files and guides for deploying Project-S to Azure using GitHub Actions and Kubernetes.

## 📁 Files Overview

### 1. **CI-CD-SETUP.md** ⭐ START HERE
Complete step-by-step guide for:
- Creating GitHub Secrets
- Setting up Azure resources (ACR, AKS, databases)
- Configuring Kubernetes manifests
- Troubleshooting deployment issues

**👉 Read this first before deploying**

### 2. **CI-CD-SUMMARY.md** 📊 REFERENCE
High-level overview of:
- Workflow execution flow
- Files created and their purposes
- Quick start checklist
- Monitoring and debugging commands
- Environment variables management

**Use this as a quick reference during deployment**

### 3. **setup-ci-cd.sh** 🛠️ AUTOMATION
Bash script that automates initial setup:
- Azure login and resource group creation
- Service Principal creation
- ACR and AKS cluster creation
- Generates GitHub Secrets list

**Run this to save time on initial setup**

```bash
chmod +x setup-ci-cd.sh
./setup-ci-cd.sh
```

### 4. **Environment File Templates**

When the CI/CD workflow runs, it creates these files:

- **`.env.backend`** - Backend service environment variables
  - Database credentials
  - Redis/RabbitMQ configuration
  - JWT token key

- **`.env.frontend`** - Frontend environment configuration
  - API URL
  - Environment name

These are automatically generated from GitHub Secrets and used during deployment.

## 🚀 Quick Start (TL;DR)

### Option A: Automated Setup (Recommended)
```bash
# 1. Make script executable
chmod +x deployment/setup-ci-cd.sh

# 2. Run setup script
./deployment/setup-ci-cd.sh

# 3. Copy secrets from output to GitHub

# 4. Push to dev/main branch
git push origin dev
```

### Option B: Manual Setup
```bash
# 1. Read CI-CD-SETUP.md completely

# 2. Add all required GitHub Secrets

# 3. Create Azure resources:
az group create --name project-s --location eastus
az acr create --resource-group project-s --name projectsregistry --sku Basic
az aks create --resource-group project-s --name project-s-aks --node-count 3

# 4. Push code to trigger workflow
git push origin dev
```

## 📋 Deployment Checklist

### Pre-Deployment
- [ ] Read CI-CD-SETUP.md
- [ ] All GitHub Secrets configured
- [ ] Azure resources created (ACR, AKS, databases)
- [ ] Local tests passing (`dotnet test`, `npm test`)
- [ ] Code pushed to `dev` or `main` branch

### Deployment
- [ ] GitHub Actions workflow triggered
- [ ] All tests passed (green checkmarks)
- [ ] Docker images built and pushed to ACR
- [ ] Pods deployed to AKS
- [ ] Services accessible

### Post-Deployment
- [ ] All pods running: `kubectl get pods -n project-s-prod`
- [ ] Gateway responding: `curl https://project-s.azurewebsites.net/api/health`
- [ ] User flows tested (login, register, etc.)
- [ ] Logs checked for errors
- [ ] Monitoring alerts configured

## 🔍 Monitoring Commands

```bash
# Check pod status
kubectl get pods -n project-s-prod

# View pod logs
kubectl logs <pod-name> -n project-s-prod --tail=50

# Watch deployment progress
kubectl rollout status deployment/project-s-gateway -n project-s-prod

# Check resource usage
kubectl top pods -n project-s-prod

# Port forward to test locally
kubectl port-forward svc/project-s-gateway 5000:80 -n project-s-prod
```

## 🆘 Getting Help

1. **Workflow fails during testing**
   - Check test logs in GitHub Actions
   - Run tests locally: `dotnet test` / `npm test`
   - See "Common Pitfalls" in [AGENTS.md](../AGENTS.md)

2. **Docker image build fails**
   - Verify Dockerfile exists in service directory
   - Check Docker build output in workflow logs
   - Ensure all dependencies in .csproj / package.json

3. **Pods crash after deployment**
   - View pod logs: `kubectl logs <pod-name> -n project-s-prod`
   - Check environment variables: `kubectl set env pod/<pod> --list`
   - Verify database connectivity

4. **Cannot connect to services**
   - Check Ingress: `kubectl get ingress -n project-s-prod`
   - Check Services: `kubectl get svc -n project-s-prod`
   - Verify network policies: `kubectl get networkpolicies -n project-s-prod`

5. **Still stuck?**
   - See Troubleshooting in CI-CD-SETUP.md
   - Check Azure Portal for resource errors
   - Review [AGENTS.md](../AGENTS.md) for architecture help

## 📚 Related Documentation

- [AGENTS.md](../AGENTS.md) - Project architecture and development guidelines
- [.github/workflows/ci-cd-azure.yml](../.github/workflows/ci-cd-azure.yml) - Main workflow file
- [Azure CLI Reference](https://docs.microsoft.com/en-us/cli/azure/reference-index)
- [Kubernetes Documentation](https://kubernetes.io/docs/)

## 🔐 Security Best Practices

✅ **DO:**
- Store all secrets in GitHub Secrets, never in code
- Use least-privilege Service Principal
- Enable network policies in Kubernetes
- Use TLS certificates for Ingress
- Rotate secrets regularly
- Enable audit logging in AKS

❌ **DON'T:**
- Commit `.env` files with real secrets
- Share AZURE_CREDENTIALS with others
- Use admin credentials for regular operations
- Allow all traffic (use NetworkPolicies)
- Deploy without resource limits

## 🎯 Typical Deployment Timeline

| Activity | Time |
|----------|------|
| Setup Azure resources (one-time) | 20-30 min |
| Configure GitHub Secrets | 10 min |
| Apply Kubernetes manifests | 5 min |
| First deploy (dev) | 10 min |
| Tests + build + deploy (prod) | 15 min |
| Post-deployment verification | 10 min |
| **Total First Deployment** | **~90 min** |
| Subsequent deployments | 10-15 min |

## 📞 Support

For questions or issues:
1. Check CI-CD-SETUP.md Troubleshooting section
2. Review GitHub Actions workflow logs
3. Check Azure Portal for resource status
4. See AGENTS.md for architecture questions

---

**Last Updated:** April 18, 2026  
**Status:** Ready for Deployment ✅
