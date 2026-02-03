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

### Create cluster
```bash
make k8s-up
```
### Apply manifests
```bash
make k8s-apply
make k8s-status
```
Access apps on localhost (no Services yet)
Run each command in its own terminal:

Frontend:
```bash
make k8s-pf-front
http://localhost:3000
```
Backend API:
```bash
make k8s-pf-api
http://localhost:5001
```
CMS:
```bash
make k8s-pf-cms
http://localhost:3333
```
Debug
```bash
make k8s-status
make k8s-front-describe
make k8s-logs-api
make k8s-logs-front
make k8s-logs-cms
```
Cleanup
```bash
make k8s-delete
make k8s-down
```