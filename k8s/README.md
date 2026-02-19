# RealEstate Kubernetes Manifests (kustomize) — Blue/Green Ready

This folder contains Kubernetes manifests for the RealEstate Portal.
Manifests are managed via **kustomize** with:
- `base/` — common resources
- `overlays/` — environment-specific patches and secrets
- The secrets are located in Ansible`s vault

This repo supports **Blue/Green deployment** using dedicated overlays:
- `overlays/prod-blue`
- `overlays/prod-green`

A typical production flow:
1) Deploy the new version to **green**
2) Verify health
3) Switch traffic to green
4) Deploy the same version to **blue** (or keep as fallback)


## Folder structure
- `base/`
    - `namespace.yaml` — `realestate` namespace
    - `api/` — deployment, service, configmap
    - `frontend/` — deployment, service
    - `cms/` — deployment, service
    - `mongo/` — stateful resources (service + storage)
    - `ingress/ingress.yaml` — base ingress definition (optional, mostly overridden in overlays)
    - `kustomization.yaml`
- `overlays/`
    - `dev/` — local/dev settings + plaintext secrets
    - `prod/` — production defaults + encrypted secrets references
    - `prod-blue/` — selects prod config but targets BLUE stack
    - `prod-green/` — selects prod config but targets GREEN stack


## Prerequisites (local)
- kubectl
- kustomize (or `kubectl apply -k`)
- Docker + k3d (for local cluster)
- Make targets from repository root


## Local: bring up Kubernetes from scratch (k3d)
From repository root:

### 1) Create cluster
```bash
make k8s-up
make k8s-context
```
### 2) Validate manifests (dry-run)
```bash
make k8s-validate
```
### 3) Apply dev overlay
```bash
make k8s-reset-dev
```
### 4) Check status and URLs
```bash
make k8s-status
make k8s-urls
```
### 5) Logs (optional)
```bash
make k8s-logs-api
make k8s-logs-frontend
make k8s-logs-cms
make k8s-logs-mongo
```
### Production: deploy via Make (recommended)
Production deploy is orchestrated via Terraform + Ansible (not manual kubectl).

From repository root:

### Full infra + app deploy (BLUE)
```bash
make deploy-blue
```
App-only redeploy (BLUE)
```bash
make deploy-app-blue
```
Same for green:
```bash
make deploy-green
make deploy-app-green
```
### Verifying what is actually deployed (recommended)
When “deploy succeeded but UI didn’t change”, verify the live HTML from inside the cluster:

### Example: BLUE cluster port-forward (frontend)
```bash
kubectl --kubeconfig .kube/realestate-prod-blue.yaml -n realestate \
  port-forward svc/realestate-frontend-svc 18080:80
  
curl -s http://127.0.0.1:18080 | grep -n "New text for autodeploy test" || true
```
This avoids confusion with:
- load balancer routing to the other stack
- DNS propagation
- browser cache

## Common issues (based on real incidents)

### 1) You updated BLUE but you are still seeing GREEN
If traffic switching is external (LB / DNS), you can deploy blue successfully but users still hit green.
Always verify:

- port-forward inside the stack
- or expose a stack-specific header in ingress for debugging

### 2) Secrets format mismatch (dev vs prod)
- `overlays/dev` uses plaintext secrets (simple local usage)
- `overlays/prod` uses encrypted/managed secrets (vaulted in Ansible)

If kustomize fails to build:

- check that the overlay expects a secret file that exists (e.g. `secret.yaml` vs `secret.enc.yaml`)
- confirm Ansible renders or applies the prod secrets correctly

### 3) Ingress does not work / ports conflict
k3s may ship Traefik and it can conflict with ingress-nginx.
Fix:

- disable Traefik in k3s
- verify ports 80/443 are free
- confirm ingress-nginx service is LoadBalancer where expected
