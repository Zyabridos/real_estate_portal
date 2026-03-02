# RealEstate Infrastructure — Terraform (Hetzner) — Blue/Green

This folder provisions production infrastructure on Hetzner for two stacks:
- **prod-blue**
- **prod-green**

Terraform is responsible for:
- VMs (k3s server + optional worker)
- firewall / networking
- outputs that generate Ansible inventory

The resulting inventory is written to:
`infrastructure/ansible/inventories/generated/inventory.prod-<stack>.ini`

## Prerequisites
- terraform
- credentials for your cloud provider (Hetzner), configured locally
- Make targets available from repository root


## Workspaces and stacks
Stacks map to Terraform workspaces:
- `prod-blue`
- `prod-green`

Make variables:
- `ENV=prod`
- `STACK=blue|green`
- `TF_WORKSPACE=$(ENV)-$(STACK)`


## Common workflows
### Prepare variables
Copy example files (from root directory):
```bash
mv ./infrastructure/terraform/terraform.tfvars.example ./infrastructure/terraform/terraform.tfvars
mv ./infrastructure/terraform/terraform.prod.blue.tfvars.example ./infrastructure/terraform/terraform.prod.blue.tfvars
mv ./infrastructure/terraform/terraform.prod.blue.tfvars.example ./infrastructure/terraform/terraform.prod.green.tfvars 
```
Set correct values:
- `terraform.tfvars` - shared variables
- `terraform.prod.blue.tfvars` - special variables (specfied based on eviorment and stack)
### Initialize (from root directory `./`)
```bash
make tf-init
```
### Plan
```bash
make tf-plan ENV=prod STACK=blue
make tf-plan ENV=prod STACK=green
```
### Apply
```bash
make tf-apply ENV=prod STACK=blue
make tf-apply ENV=prod STACK=green
```
### Outputs
```bash
make tf-output ENV=prod STACK=blue
```
### Destroy (danger)
```bash
make tf-destroy ENV=prod STACK=blue
```
### Inventory output (important)
Terraform emits an output named ansible_inventory_ini.
Make uses it to write the generated inventory file:
```bash
make ansible-inventory ENV=prod STACK=blue
```
Expected file:
`infrastructure/ansible/inventories/generated/inventory.prod-blue.ini`

## Common issues
### 1) “Ansible runs but no hosts matched”
Cause: inventory groups don’t match the Ansible playbook.
Your playbook expects groups like:

- `k3s_server`
- `k3s_workers`

So Terraform output must generate those groups,
or you must adapt the playbook.

### 2) “Deploy to blue somehow touched green”
Cause: Make workspace selection or target override bug.
Always confirm:

- `workspace=prod-blue appears in output`
- generated inventory file ends with `inventory.prod-blue.ini`

### 3) SSH known_hosts / host key changes
If servers are recreated, SSH host keys change.
Make can refresh known_hosts using the inventory:
```bash
make ansible-known-hosts ENV=prod STACK=blue
```