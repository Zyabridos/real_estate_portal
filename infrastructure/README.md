
# Infrastructure (Terraform + Ansible)

This directory contains infrastructure provisioning and deployment automation for the RealEstate Portal.

- **Terraform** provisions servers and networking/security (cloud resources).
- **Ansible** configures hosts and deploys the application (Docker Compose).


## Prerequisites

Install locally:

- Terraform
- Ansible
- SSH access to provisioned hosts (SSH key)ss

## Quick start

### 1) Provision infrastructure (Terraform)

From repo root:

```bash
make infra-init
make infra-plan
make infra-apply
make infra-output
```
Common options:

```bash
make infra-plan TF_WORKSPACE=prod
make infra-apply TF_WORKSPACE=prod
make infra-plan TF_WORKSPACE=prod TFVARS=env/prod.tfvars
```

2) Configure & deploy (Ansible)
   Run a dry-run first:
```bash
make infra-ansible-dry-run ANSIBLE_LIMIT=all
```
Run the full playbook:
```bash
make infra-ansible-playbook ANSIBLE_LIMIT=all
```
Deploy only (tagged tasks):
```bash
make deploy ANSIBLE_LIMIT=all
```

## Secrets (Ansible Vault)
This project uses Ansible Vault for secrets.

Make expects a local file with the vault password:

`vault-password` (repo root by default)

Create it locally:
```bash
echo "provided-password" > vault-password
chmod 600 vault-password
```
Typical vault file location:

`infrastructure/ansible/group_vars/prod/vault.yml` (encrypted)

Encrypt:
```bash
cd infrastructure/ansible
ansible-vault encrypt group_vars/prod/vault.yml --vault-password-file ../../vault-password
```
Edit:
```bash
cd infrastructure/ansible
ansible-vault edit group_vars/prod/vault.yml --vault-password-file ../../vault-password
```
### SSH helper
If you just want a quick SSH into a host
```bash:
make infra-ssh INFRA_SSH_HOST=1.2.3.4 INFRA_SSH_USER=root
```
## Notes & conventions

-   Avoid hardcoding IPs in Makefiles/README. Use Terraform outputs and inventory files.
-   Keep environment separation (`dev`, `prod`) via:
    -   Terraform workspaces and/or `*.tfvars`
    -   Ansible `group_vars/<env>/` and vault files