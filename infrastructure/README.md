# RealEstate Infrastructure (Terraform + Ansible + Kubernetes) — Blue/Green

This folder contains everything required to provision and deploy the RealEstate Portal:
- **Terraform** provisions Hetzner infrastructure (VMs, networks, firewall, shared Load Balancer)
- **Ansible** bootstraps **k3s**, installs cluster addons (Helm), deploys Kubernetes manifests (kustomize overlays)
- **Kubernetes manifests** live in `../k8s` (base + overlays)
- Deployment supports **Blue/Green** via `STACK=blue|green`

> Source of truth for orchestration is the **root Makefile**:
> - infra + app: `make deploy ENV=prod STACK=blue`
> - app only: `make deploy-app ENV=prod STACK=blue`

---

## Directory layout

- `infrastructure/terraform/` — Hetzner infra (workspaces: `prod-blue`, `prod-green`)
- `infrastructure/ansible/` — k3s bootstrap + k8s deploy/verify
- `k8s/` (repo root) — kustomize base/overlays (blue/green overlays)
- `.kube/` (repo root, generated) — kubeconfigs fetched by Ansible:
  - `.kube/realestate-prod-blue.yaml`
  - `.kube/realestate-prod-green.yaml`

---

## Prerequisites (local)

Required:
- `terraform`
- `ansible` (+ collections from `infrastructure/ansible/requirements.yml`)
- `kubectl`
- `helm`
- SSH access to servers (private key loaded or available)

Required local file:
- `infrastructure/ansible/vault-password`

Recommended:
- `make` (all workflows are Make targets)

---

## Quickstart (Production)

### 1) Full deploy (infra + app)
Deploy BLUE:
```bash
make deploy ENV=prod STACK=blue
# or shortcut:
make deploy-blue
````
### Deploy GREEN
```bash
make deploy ENV=prod STACK=green
# or shortcut:
make deploy-green
```
## What it does:
1. `tf-init` + `tf-apply` for the selected workspace `(TF_WORKSPACE=$(ENV)-$(STACK))`
2. Generates Ansible inventory from Terraform output
3. Refreshes ~/.ssh/known_hosts from inventory
4. Runs Ansible playbook:
   - installs k3s server/agents
   - fetches & patches kubeconfig to use public endpoint
   - installs addons (ingress-nginx, cert-manager, metrics-server)
   - applies kustomize overlay from k8s/overlays/<env>-<stack>
   - applies vaulted secrets
   - verifies workloads + in-cluster health check

## 2) Inventory generation (important)

Terraform creates an output `ansible_inventory_ini`.
After `tf-apply`, Make uses it to write the generated inventory file:

- infrastructure/ansible/inventories/generated/inventory.prod-blue.ini
- infrastructure/ansible/inventories/generated/inventory.prod-green.ini

## 3) App-only redeploy (no Terraform changes)

BLUE:
```bash
make deploy-app ENV=prod STACK=blue
# or shortcut:
make deploy-app-blue
```
GREEN:
```bash
make deploy-app ENV=prod STACK=green
# or shortcut:
make deploy-app-green
```

## Notes

During development, I occasionally ran into an ingress port conflict between the default k3s Traefik and ingress-nginx.
k3s may ship Traefik enabled by default, and it can compete for ports 80/443, preventing ingress-nginx from binding properly.

Check what is currently listening on ports 80/443 and detect common ingress components:
```bash
make k8s-ingress-debug
```
If Traefik is the culprit, remove the default k3s Traefik and restart k3s:
```bash
make k8s-traefik-remove
```
## See also:
- infrastructure/terraform/README.md (LB + network + switching)
- infrastructure/ansible/README.md (roles and deeper debug)
- k8s/README.md (manifests & overlays)