# Kubernetes 

This folder contains a minimal Kubernetes setup for the RealEstate Portal:

**Note:** I am learning by doing, so the structure is not final. It probably will change a lot, but one step at the time.

## Project structure
- `k8s/frontend/deployment.yaml`
- `k8s/api/deployment.yaml`
- `k8s/cms/deployment.yaml`

Each file contains:
- `Deployment`
- `HorizontalPodAutoscaler` (HPA)

## Prerequisites
- `k3d`
- `kubectl`
- Docker

## Make targets
All commands are defined in `make/k8s.mk`.

## Start from scratch (highly recommended)

### 1) Create the k3d cluster

```bash
make k8s-up
```
This creates cluster realestate and exposes ports:

3000, 5001, 3333 on the k3d load balancer.

### 2) Apply manifests
```bash
make k8s-apply
```
### 3) Wait until everything is ready
```bash
make k8s-wait-all
```
### 4) Check status
```bash
make k8s-status
make k8s-urls
```
## Verify locally
### For Frontend open:
```
http://localhost:3000
```
### For backend ping health-cheak:
```
curl -i http://localhost:5001/api/health
```
**Seeding data**

MongoDB is ephemeral in this setup.
If you recreate the cluster, you must seed again.

Seed brokers + properties into the API:

```bash
BACKEND_URL=http://localhost:5001 make seed-brokers
BACKEND_URL=http://localhost:5001 make seed-properties
````
Check that there is some data. For example, send request to:
```bash
curl -i "http://localhost:5001/api/properties?page=1&pageSize=5"
```
### CMS
Open:
```
http://localhost:3333
```

## Logs & debugging
API logs:
```bash
make k8s-logs-api
```
Mongo logs:
```bash
make k8s-logs-mongo
```
Cluster resources:
```bash
make k8s-status
kubectl get all -l project=realestate
```

### Ingress (optional)

This setup uses **NGINX Ingress Controller** for local HTTP routing:

- `/` → frontend service
- `/api` and `/health` → API service
- `cms.localhost` → Sanity Studio
### Prerequisite
 Install **ingress-nginx** in your cluster. [Quick Start](https://kubernetes.github.io/ingress-nginx/deploy/#quick-start)

Apply ingress rules:

```bash
kubectl apply -f k8s/ingress/ingress.yaml
kubectl get ingress
```