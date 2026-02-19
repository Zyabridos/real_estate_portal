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

Required local file (never commit):
- `infrastructure/ansible/vault-password`

Recommended:
- `make` (all workflows are Make targets)

---

## 1) Quickstart (Production)

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
1. tf-init + tf-apply for the selected workspace (TF_WORKSPACE=$(ENV)-$(STACK))
2. Generates Ansible inventory from Terraform output
3. Refreshes ~/.ssh/known_hosts from inventory
4. Runs Ansible playbook:
   - installs k3s server/agents
   - fetches & patches kubeconfig to use public endpoint
   - installs addons (ingress-nginx, cert-manager, metrics-server)
   - applies kustomize overlay from k8s/overlays/<env>-<stack>
   - applies vaulted secrets
   - verifies workloads + in-cluster health check
## 2) App-only redeploy (no Terraform changes)

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
### Useful Make targets
**Infra (Terraform)**
```bash
make tf-init
make tf-plan   ENV=prod STACK=blue
make tf-apply  ENV=prod STACK=blue
make tf-output ENV=prod STACK=blue
make tf-destroy ENV=prod STACK=blue   # dangerous
```
**Ansible (debug)**
```bash
make ansible-inventory   ENV=prod STACK=blue
make ansible-known-hosts ENV=prod STACK=blue
make ansible-ping        ENV=prod STACK=blue ANSIBLE_LIMIT=all
make ansible-show-diff   ENV=prod STACK=blue   # --check --diff
make ansible-playbook    ENV=prod STACK=blue
```
**k3s / ingress helpers (SSH-based)**
```bash
make k8s-ingress-debug
make k8s-traefik-remove
```
## Blue/Green model (how it works here)
- Two stacks exist: blue and green.
- Each stack has:
  - -its own kubeconfig (.kube/realestate-prod-<stack>.yaml)
  - its own kustomize overlay (k8s/overlays/prod-<stack>)
  - its own server/worker nodes (depending on tfvars)
- Traffic switching is done at the shared Hetzner Load Balancer level by changing which stack matches the LB label selector (see infrastructure/terraform/README.md).
- Typical safe rollout:
1. Deploy new version to green
2. Verify in-cluster health + external routing
3. Switch LB traffic to green
4. Deploy same version to blue (or keep as fallback)
## Common issues (things I hit during app development)
### 1) “It deployed, but the website didn’t change”
Most common cause: Kubernetes still runs an old image.
Typical reasons:
- The deployment uses a mutable tag like :latest and does not pull a new image.
- imagePullPolicy is not Always for mutable tags.
- Registry image was never pushed (CI build did not update the tag/digest).
- You are looking at the other stack (LB still points to green while you updated blue).

Fast checks:

- Confirm which image is used:
```bash
kubectl --kubeconfig .kube/realestate-prod-blue.yaml -n realestate \
  get deploy realestate-frontend -o jsonpath='{.spec.template.spec.containers[0].image}{"\n"}'
  ```
- Confirm the new UI is reachable from inside the cluster via port-forward:
```bash
kubectl --kubeconfig .kube/realestate-prod-blue.yaml -n realestate \
  port-forward svc/realestate-frontend-svc 18080:80
curl -s http://127.0.0.1:18080 | grep -n "New text for autodeploy test" || true
```
### 2) Ingress/ports conflict (Traefik vs ingress-nginx)
k3s may ship Traefik; it can conflict with ingress-nginx on 80/443.

Check and fix:
```bash
make k8s-ingress-debug
make k8s-traefik-remove
```
## 3) cert-manager not issuing certs / HTTPS not working

Check:
```bash
kubectl --kubeconfig .kube/realestate-prod-blue.yaml -n cert-manager get pods
kubectl --kubeconfig .kube/realestate-prod-blue.yaml -n realestate get certificate,secret,certificaterequest,order,challenge -o wide
```

## See also:
- infrastructure/terraform/README.md (LB + network + switching)
- infrastructure/ansible/README.md (roles and deeper debug)
- k8s/README.md (manifests & overlays)