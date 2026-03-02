# RealEstate — Ansible (k3s bootstrap + k8s deploy) — Blue/Green

This folder bootstraps **k3s** on Hetzner VMs and deploys Kubernetes manifests from `../../k8s`
using **kustomize overlays**. Blue/Green is controlled via `STACK=blue|green`.

Runs mostly from the **local runner** (your laptop):
- installs k3s on remote nodes
- fetches kubeconfig to repo root `.kube/`
- installs cluster addons via Helm (from localhost)
- applies kustomize overlays + secrets (from localhost)

> Source of truth for orchestration is the **./make/vars/infra** and **./make/vars/images**.  
> Make calls Ansible with `ANSIBLE_CONFIG=infrastructure/ansible/ansible.cfg`.

---

## Directory layout

- `ansible.cfg` — local Ansible config (roles_path, inventory defaults, etc.)
- `playbooks/site.yml` — main orchestration playbook
- `inventories/generated/` — generated inventory + group vars:
    - `inventory.prod-blue.ini`
    - `inventory.prod-green.ini`
    - `group_vars/`
        - `all.yml`
        - `prod.yml`
        - `prod-blue.yml`
        - `prod-green.yml`
- `roles/`
    - `common` — base packages on all nodes
    - `k3s_server` — install k3s server, read node token
    - `k3s_agent` — install k3s agents
    - `kubeconfig` — fetch kubeconfig & patch server endpoint
    - `k8s_addons` — Helm addons (ingress-nginx, cert-manager, metrics-server)
    - `k8s_deploy` — apply kustomize overlay + secrets + pin images by tag
    - `k8s_verify` — verify pods/services + health checks
- `secrets/` — vaulted Kubernetes secrets (per env):
    - `secrets/dev/*.vault.yaml`
    - `secrets/prod/*.vault.yaml`
- `vault-password` — local vault pass file (never commit)

---

## Prerequisites (local)

- `ansible`
- `kubectl`
- `helm`
- vault password file:
    - `infrastructure/ansible/vault-password`

## Install collections:
### Run via Make (recommended)
App deploy (no Terraform)

#### Blue (default):
```bash
make deploy-app
# explicit:
make deploy-app ENV=prod STACK=blue
```
#### Green:
```bash
make deploy-app ENV=prod STACK=green
```
#### Dry-run (check + diff)
```bash
make ansible-show-diff
```
### Note: 
> In `--check` mode, kubectl apply/pin/verify steps are skipped.
The playbook prints a “what would happen” summary instead.

### SSH / inventory helpers

Refresh ~/.ssh/known_hosts from generated inventory:
```bash
make ansible-known-hosts
```
Ping hosts:
```bash
make ansible-ping
```
## Key variables (where they live)
### Make (repo root)
These are your main entry points:

- `ENV` (default: `prod`)
- `STACK` (default: `blue`)
- `IMAGE_TAG` (default: `sha-<git-short-sha>`)
- `ANSIBLE_LIMIT` (default: `all`)
- `EXTRA_VARS` (optional overrides)

### Make always passes Ansible extra-vars:

- `env=<ENV>`
- `stack_id=<STACK>`
- `image_tag=<IMAGE_TAG>`

### Secrets (vaulted)

Applied by `k8s_deploy`:

- `secrets/<env>/mongo-secret.vault.yaml`
- `secrets/<env>/api-secret.vault.yaml`

### What the playbook does (high-level)

1) Base packages on all nodes (`common`)

2) Install k3s server (`k3s_server`)

3) Install k3s agents (`k3s_agent`)

4) Fetch kubeconfig to repo root:

   - `.kube/realestate-<env>-<stack>.yaml`

   - patches server endpoint to public IP (k3s default is `127.0.0.1`)

5) Install addons via Helm (from localhost)

6) Deploy manifests via kustomize overlay:

   - `k8s/overlays/<env>-<stack>`

7) Apply vaulted secrets

8) Pin images by single immutable `image_tag`

9) Verify rollout / health (`k8s_verify`)

### Tag policy (important)

Production deploy requires an immutable tag.

Default tag is generated automatically:

- `IMAGE_TAG=sha-<git-short-sha>`

If you want to deploy a specific tag:
```bash
make deploy-app IMAGE_TAG=sha-deadbeef
```