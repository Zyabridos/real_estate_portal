# Directories
INFRA_DIR     ?= infrastructure
TERRAFORM_DIR ?= $(INFRA_DIR)/terraform
ANSIBLE_DIR   ?= $(INFRA_DIR)/ansible

# Tools
TF                    ?= terraform
ANSIBLE               ?= ansible
ANSIBLE_MAIN_PLAYBOOK ?= playbooks/site.yml

# Env selection
ENV   ?= prod
STACK ?= blue

TF_WORKSPACE ?= $(ENV)-$(STACK)
TFVARS       ?= terraform.$(ENV).$(STACK).tfvars
TFVARS_PATH  := $(abspath $(TERRAFORM_DIR)/$(TFVARS))

IMAGE_TAG ?= latest

# Ansible options
ANSIBLE_LIMIT   ?= all
EXTRA_VARS      ?=
VAULT_PASS_FILE ?= vault-password

# Inventory file (generated)
ANSIBLE_INVENTORY      ?= inventories/generated/inventory.$(ENV)-$(STACK).ini
ANSIBLE_INVENTORY_PATH := $(ANSIBLE_DIR)/$(ANSIBLE_INVENTORY)

# SSH helpers
SSH_KEYSCAN      ?= ssh-keyscan
SSH_KEYGEN       ?= ssh-keygen
KNOWN_HOSTS_FILE ?= $(HOME)/.ssh/known_hosts

INFRA_SSH_USER ?= root
INFRA_SSH_HOST ?=

# K3S 
K3S_SERVER_IP ?= 89.167.61.6

# Helpers
define TF_SELECT_WORKSPACE
$(TF) -chdir=$(TERRAFORM_DIR) workspace select $(TF_WORKSPACE) >/dev/null 2>&1 || \
$(TF) -chdir=$(TERRAFORM_DIR) workspace new $(TF_WORKSPACE) >/dev/null
endef

define ensure_vault
test -f "$(ANSIBLE_DIR)/$(VAULT_PASS_FILE)" || \
( echo -e "$(RED)Vault password file not found: $(ANSIBLE_DIR)/$(VAULT_PASS_FILE)$(RESET)"; exit 1 )
endef

tf-init:
	@echo -e "$(BLUE)▶ Terraform init ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) init

tf-format:
	@echo -e "$(BLUE)▶ Terraform formatting (pretty-print) ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) fmt -recursive

tf-validate:
	@echo -e "$(BLUE)▶ Terraform validate ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) validate

tf-plan:
	@echo -e "$(BLUE)▶ Terraform plan (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) plan -var-file="$(TFVARS_PATH)"

tf-apply:
	@echo -e "$(BLUE)▶ Terraform apply (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@echo -e "  $(YELLOW)workspace:$(RESET) $$($(TF) -chdir=$(TERRAFORM_DIR) workspace show)"
	@$(TF) -chdir=$(TERRAFORM_DIR) apply -auto-approve -var-file="$(TFVARS_PATH)"

tf-destroy:
	@echo -e "$(BLUE)▶ Terraform destroy (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) destroy -auto-approve -var-file="$(TFVARS_PATH)"

tf-output:
	@echo -e "$(BLUE)▶ Terraform output (workspace: $(TF_WORKSPACE))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) output

# Inventory generation
.PHONY: ansible-inventory ansible-known-hosts

ansible-inventory:
	@echo -e "$(BLUE)▶ Generate Ansible inventory from Terraform output (workspace: $(TF_WORKSPACE))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@mkdir -p "$(ANSIBLE_DIR)/inventories/generated"
	@$(TF) -chdir="$(TERRAFORM_DIR)" output -raw ansible_inventory_ini > "$(ANSIBLE_INVENTORY_PATH)"
	@test -s "$(ANSIBLE_INVENTORY_PATH)" || (echo -e "$(RED)Inventory file is empty. Terraform output ansible_inventory_ini is missing/empty.$(RESET)"; exit 1)
	@echo -e "$(GREEN)✔ wrote:$(RESET) $(ANSIBLE_INVENTORY_PATH)"
	@sed -n '1,120p' "$(ANSIBLE_INVENTORY_PATH)"

ansible-known-hosts: ansible-inventory
	@echo -e "$(BLUE)▶ Update SSH known_hosts from inventory ($(ANSIBLE_INVENTORY))$(RESET)"
	@mkdir -p "$(HOME)/.ssh"
	@chmod 700 "$(HOME)/.ssh"
	@touch "$(KNOWN_HOSTS_FILE)"
	@chmod 600 "$(KNOWN_HOSTS_FILE)"
	@{ grep -Eo '([0-9]{1,3}\.){3}[0-9]{1,3}' "$(ANSIBLE_INVENTORY_PATH)" || true; } | sort -u | \
	while read -r ip; do \
		[ -z "$$ip" ] && continue; \
		echo -e "  $(YELLOW)- refreshing host key for$(RESET) $$ip"; \
		$(SSH_KEYGEN) -R "$$ip" >/dev/null 2>&1 || true; \
		$(SSH_KEYSCAN) -H -t ed25519,rsa $$ip 2>/dev/null >> "$(KNOWN_HOSTS_FILE)" || true; \
	done

# Ansible
.PHONY: ansible-ping ansible-playbook ansible-show-diff ansible-deploy

ansible-ping: ansible-known-hosts
	@echo -e "$(BLUE)▶ Ansible ping (limit: $(ANSIBLE_LIMIT))$(RESET)"
	@cd "$(ANSIBLE_DIR)" && \
	$(ANSIBLE) -i "$(ANSIBLE_INVENTORY)" "$(ANSIBLE_LIMIT)" -m ping

ansible-playbook: ansible-known-hosts
	@echo -e "$(BLUE)▶ Ansible playbook (workspace: $(TF_WORKSPACE), limit: $(ANSIBLE_LIMIT))$(RESET)"
	@$(ensure_vault)
	@cd "$(ANSIBLE_DIR)" && \
	ansible-playbook -i "$(ANSIBLE_INVENTORY)" "$(ANSIBLE_MAIN_PLAYBOOK)" \
		--limit "$(ANSIBLE_LIMIT)" \
		$(if $(EXTRA_VARS),-e "$(EXTRA_VARS)",) \
		--vault-password-file "$(VAULT_PASS_FILE)"

ansible-show-diff: ansible-known-hosts
	@echo -e "$(BLUE)▶ Ansible dry-run (workspace: $(TF_WORKSPACE), limit: $(ANSIBLE_LIMIT))$(RESET)"
	@$(ensure_vault)
	@cd "$(ANSIBLE_DIR)" && \
	ansible-playbook -i "$(ANSIBLE_INVENTORY)" "$(ANSIBLE_MAIN_PLAYBOOK)" \
		--check --diff \
		--limit "$(ANSIBLE_LIMIT)" \
		$(if $(EXTRA_VARS),-e "$(EXTRA_VARS)",) \
		--vault-password-file "$(VAULT_PASS_FILE)"

# Deploy app
.PHONY: deploy-app
deploy-app: ansible-inventory ansible-known-hosts ansible-playbook
	@echo -e "$(GREEN)App deploy done: workspace=$(TF_WORKSPACE)$(RESET)"

deploy-app-blue:
	@$(MAKE) deploy-app ENV=prod STACK=blue EXTRA_VARS='env=prod stack_id=blue image_tag=$(IMAGE_TAG)'

deploy-app-green:
	@$(MAKE) deploy-app ENV=prod STACK=green EXTRA_VARS='env=prod stack_id=green image_tag=$(IMAGE_TAG)'

# One-command deploy (terraform + ansible) - I might delete servers at some point, so nice to have
.PHONY: deploy deploy-blue deploy-green

deploy: tf-init tf-apply deploy-app
	@echo -e "$(GREEN)Deploy done: workspace=$(TF_WORKSPACE)$(RESET)"

deploy-blue:
	@$(MAKE) deploy ENV=prod STACK=blue

deploy-green:
	@$(MAKE) deploy ENV=prod STACK=green

# Debug / SSH
.PHONY: show-infra-config infra-ssh

show-infra-config:
	@echo -e "$(BOLD)ENV$(RESET)=$(ENV)"
	@echo -e "$(BOLD)STACK$(RESET)=$(STACK)"
	@echo -e "$(BOLD)TF_WORKSPACE$(RESET)=$(TF_WORKSPACE)"
	@echo -e "$(BOLD)TFVARS$(RESET)=$(TFVARS)"
	@echo -e "$(BOLD)TFVARS_PATH$(RESET)=$(TFVARS_PATH)"
	@echo -e "$(BOLD)ANSIBLE_DIR$(RESET)=$(ANSIBLE_DIR)"
	@echo -e "$(BOLD)ANSIBLE_MAIN_PLAYBOOK$(RESET)=$(ANSIBLE_MAIN_PLAYBOOK)"
	@echo -e "$(BOLD)ANSIBLE_INVENTORY$(RESET)=$(ANSIBLE_INVENTORY)"
	@echo -e "$(BOLD)ANSIBLE_INVENTORY_PATH$(RESET)=$(ANSIBLE_INVENTORY_PATH)"
	@echo -e "$(BOLD)ANSIBLE_LIMIT$(RESET)=$(ANSIBLE_LIMIT)"
	@echo -e "$(BOLD)VAULT_PASS_FILE$(RESET)=$(VAULT_PASS_FILE)"
	@echo -e "$(BOLD)INFRA_SSH_USER$(RESET)=$(INFRA_SSH_USER)"
	@echo -e "$(BOLD)INFRA_SSH_HOST$(RESET)=$(INFRA_SSH_HOST)"
	@echo -e "$(BOLD)K3S_SERVER_IP$(RESET)=$(K3S_SERVER_IP)"

infra-ssh:
	@test -n "$(INFRA_SSH_HOST)" || (echo -e "$(RED)Set INFRA_SSH_HOST=<ip>$(RESET)"; exit 1)
	@echo -e "$(BLUE)▶ SSH:$(RESET) $(INFRA_SSH_USER)@$(INFRA_SSH_HOST)"
	@ssh "$(INFRA_SSH_USER)@$(INFRA_SSH_HOST)"

# --- Kubernetes / k3s helpers ---

.PHONY: k8s-ingress-debug k8s-traefik-remove

k8s-ingress-debug: ## Debug ingress port conflicts on k3s server (checks 80/443 and common ingress pods - i.e. Traefik headache)
	@ssh root@$(K3S_SERVER_IP) "ss -lntp | egrep ':80|:443' || true"
	@ssh root@$(K3S_SERVER_IP) "kubectl -n kube-system get pods | egrep 'traefik|caddy|nginx' || true"

k8s-ingress-status: ## Show ingress-nginx status: nodes, ds/pods/events (I used this command a couple of times when controller was "Pending")
	@ssh root@$(K3S_SERVER_IP) "kubectl get nodes -o wide || true"
	@ssh root@$(K3S_SERVER_IP) "kubectl -n ingress-nginx get ds,deploy,svc,pods -o wide 2>/dev/null || true"
	@ssh root@$(K3S_SERVER_IP) "kubectl -n ingress-nginx get events --sort-by=.metadata.creationTimestamp 2>/dev/null | tail -n 40 || true"

k8s-traefik-remove: ## Remove default k3s Traefik (i.e. Traefik headache)
	@ssh root@$(K3S_SERVER_IP) "\
		kubectl -n kube-system delete helmchart traefik --ignore-not-found; \
		kubectl -n kube-system delete helmchartconfig traefik --ignore-not-found || true; \
		rm -f /var/lib/rancher/k3s/server/manifests/traefik.yaml /var/lib/rancher/k3s/server/manifests/traefik-config.yaml; \
		systemctl restart k3s \
	"
