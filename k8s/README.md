# Kubernetes 
# Kubernetes

This folder contains a minimal Kubernetes setup for the RealEstate Portal.
It is intended for local development with **k3d** and provides a clean baseline for a future production setup.

All application resources are deployed into the `realestate` namespace.
Networking is available via direct ports (debug) and optionally via an **NGINX Ingress Controller**.

## Project structure
**Note:** I am learning by doing, so the structure is not final. It probably will change a lot, but one step at the time.
- `k8s/namespace.yaml` — `realestate` namespace definition
- `k8s/mongo/` — MongoDB deployment + service
- `k8s/api/` — API deployment + service (+ HPA)
- `k8s/frontend/` — Frontend deployment + service (+ HPA)
- `k8s/cms/` — Sanity deployment + service (+ HPA)
- `k8s/ingress/` — local ingress rules (optional)

Each `deployment.yaml` typically includes:
- `Deployment`
- `HorizontalPodAutoscaler` (HPA)

## Prerequisites
- k3d
- kubectl
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

### 2) Apply namespace
```bash
kubectl apply -f k8s/namespace.yaml
```

### 3) Apply manifests
```bash
make k8s-apply
```
### 4) Wait until everything is ready
```bash
make k8s-wait-all
```
### 5) Check status
```bash
make k8s-status
make k8s-urls
```
## Verify locally
### Ingress (recommended)
```md
- Frontend: http://localhost/
- API health: curl -i http://localhost/api/health
- CMS: http://cms.localdev.me/ (or http://cms.localhost/)
```
### Direct ports (debug)
```md
- Frontend: http://localhost:3000
- API health: curl -i http://localhost:5001/api/health
- CMS: http://localhost:3333
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

 Install **ingress-nginx** in your cluster. [Quick Start](https://kubernetes.github.io/ingress-nginx/deploy/#quick-start)

Apply ingress rules:

```bash
kubectl apply -f k8s/ingress/ingress.yaml
kubectl get ingress
```