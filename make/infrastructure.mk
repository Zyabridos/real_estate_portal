# Directories
INFRA_DIR     ?= infrastructure
TERRAFORM_DIR ?= $(INFRA_DIR)/terraform
ANSIBLE_DIR   ?= $(INFRA_DIR)/ansible

# Tools
TF                   ?= terraform
ANSIBLE              ?= ansible
ANSIBLE_PLAYBOOK_BIN ?= ansible-playbook
ANSIBLE_PLAYBOOK     ?= playbook.yml

# Env selection
ENV   ?= prod
STACK ?= blue

TF_WORKSPACE ?= $(ENV)-$(STACK)
TFVARS       ?= terraform.$(ENV).$(STACK).tfvars
TFVARS_PATH  := $(abspath $(TERRAFORM_DIR)/$(TFVARS))

# Ansible options
ANSIBLE_LIMIT   ?= all
EXTRA_VARS      ?=
VAULT_PASS_FILE ?= vault-password

# Inventory file (generated)
ANSIBLE_INVENTORY      ?= inventory.$(TF_WORKSPACE).ini
ANSIBLE_INVENTORY_PATH := $(ANSIBLE_DIR)/$(ANSIBLE_INVENTORY)

# SSH helpers
SSH_KEYSCAN      ?= ssh-keyscan
SSH_KEYGEN       ?= ssh-keygen
KNOWN_HOSTS_FILE ?= $(HOME)/.ssh/known_hosts

INFRA_SSH_USER ?= root
INFRA_SSH_HOST ?=

# Colors (fallback if not defined in root Makefile)
RESET ?= \033[0m
BOLD  ?= \033[1m
GREEN ?= \033[1;32m
YELLOW?= \033[1;33m
RED   ?= \033[1;31m
BLUE  ?= \033[1;34m

# Helpers
define TF_SELECT_WORKSPACE
$(TF) -chdir=$(TERRAFORM_DIR) workspace select $(TF_WORKSPACE) >/dev/null 2>&1 || \
$(TF) -chdir=$(TERRAFORM_DIR) workspace new $(TF_WORKSPACE) >/dev/null
endef

define ensure_vault
test -f "$(ANSIBLE_DIR)/$(VAULT_PASS_FILE)" || \
( echo -e "$(RED)Vault password file not found: $(ANSIBLE_DIR)/$(VAULT_PASS_FILE)$(RESET)"; exit 1 )
endef

# Help
.PHONY: infra-help
infra-help:
	@echo -e "$(BOLD)RealEstate infra commands:$(RESET)"
	@echo ""
	@echo -e "$(YELLOW)Deploy:$(RESET)"
	@echo -e "  $(GREEN)make deploy-blue$(RESET)                - Terraform apply + inventory + known_hosts + ansible (prod-blue)"
	@echo -e "  $(GREEN)make deploy-green$(RESET)               - Terraform apply + inventory + known_hosts + ansible (prod-green)"
	@echo ""
	@echo -e "$(YELLOW)Terraform:$(RESET)"
	@echo -e "  $(GREEN)make infra-init$(RESET)                 - terraform init"
	@echo -e "  $(GREEN)make infra-plan$(RESET)                 - terraform plan (workspace: $(TF_WORKSPACE))"
	@echo -e "  $(GREEN)make infra-apply$(RESET)                - terraform apply"
	@echo -e "  $(GREEN)make infra-output$(RESET)               - terraform output"
	@echo -e "  $(GREEN)make infra-fmt$(RESET)                  - terraform fmt"
	@echo -e "  $(GREEN)make infra-validate$(RESET)             - terraform validate"
	@echo -e "  $(GREEN)make infra-destroy$(RESET)              - terraform destroy ($(RED)danger!$(RESET))"
	@echo ""
	@echo -e "$(YELLOW)Ansible:$(RESET)"
	@echo -e "  $(GREEN)make infra-inventory$(RESET)            - generate inventory from terraform output"
	@echo -e "  $(GREEN)make infra-known-hosts$(RESET)          - refresh ~/.ssh/known_hosts from inventory"
	@echo -e "  $(GREEN)make infra-ansible-ping$(RESET)         - ansible ping (limit: $(ANSIBLE_LIMIT))"
	@echo -e "  $(GREEN)make infra-ansible-playbook$(RESET)     - run full playbook"
	@echo -e "  $(GREEN)make infra-ansible-dry-run$(RESET)      - run playbook --check --diff"
	@echo -e "  $(GREEN)make infra-ansible-deploy$(RESET)       - run playbook with --tags deploy"
	@echo -e "  $(GREEN)make infra-ssh$(RESET)                  - ssh helper (INFRA_SSH_HOST=...)"
	@echo ""
	@echo -e "$(YELLOW)Config:$(RESET)"
	@echo -e "  $(GREEN)make show-config$(RESET)                - print infra config"
	@echo ""

infra-init:
	@echo -e "$(BLUE)▶ Terraform init ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) init

infra-fmt:
	@echo -e "$(BLUE)▶ Terraform fmt ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) fmt -recursive

infra-validate:
	@echo -e "$(BLUE)▶ Terraform validate ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) validate

infra-plan:
	@echo -e "$(BLUE)▶ Terraform plan (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) plan -var-file="$(TFVARS_PATH)"

infra-apply:
	@echo -e "$(BLUE)▶ Terraform apply (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@echo -e "  $(YELLOW)workspace:$(RESET) $$($(TF) -chdir=$(TERRAFORM_DIR) workspace show)"
	@$(TF) -chdir=$(TERRAFORM_DIR) apply -auto-approve -var-file="$(TFVARS_PATH)"

infra-destroy:
	@echo -e "$(BLUE)▶ Terraform destroy (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) destroy -auto-approve -var-file="$(TFVARS_PATH)"

infra-output:
	@echo -e "$(BLUE)▶ Terraform output (workspace: $(TF_WORKSPACE))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) output

# Inventory generation
.PHONY: infra-inventory infra-known-hosts

infra-inventory:
	@echo -e "$(BLUE)▶ Generate Ansible inventory from Terraform output (workspace: $(TF_WORKSPACE))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@mkdir -p "$(ANSIBLE_DIR)"
	@$(TF) -chdir="$(TERRAFORM_DIR)" output -raw ansible_inventory_ini > "$(ANSIBLE_INVENTORY_PATH)"
	@test -s "$(ANSIBLE_INVENTORY_PATH)" || (echo -e "$(RED)Inventory file is empty. Terraform output ansible_inventory_ini is missing/empty.$(RESET)"; exit 1)
	@echo -e "$(GREEN)✔ wrote:$(RESET) $(ANSIBLE_INVENTORY_PATH)"
	@sed -n '1,120p' "$(ANSIBLE_INVENTORY_PATH)"

infra-known-hosts: infra-inventory
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
.PHONY: infra-ansible-ping infra-ansible-playbook infra-ansible-dry-run infra-ansible-deploy

infra-ansible-ping: infra-known-hosts
	@echo -e "$(BLUE)▶ Ansible ping (limit: $(ANSIBLE_LIMIT))$(RESET)"
	@cd "$(ANSIBLE_DIR)" && \
	$(ANSIBLE) -i "$(ANSIBLE_INVENTORY)" "$(ANSIBLE_LIMIT)" -m ping

infra-ansible-playbook: infra-known-hosts infra-check-playbook
	@echo -e "$(BLUE)▶ Ansible playbook (workspace: $(TF_WORKSPACE), limit: $(ANSIBLE_LIMIT))$(RESET)"
	@$(ensure_vault)
	@cd "$(ANSIBLE_DIR)" && \
	$(ANSIBLE_PLAYBOOK_BIN) -i "$(ANSIBLE_INVENTORY)" "$(ANSIBLE_PLAYBOOK)" \
		--limit "$(ANSIBLE_LIMIT)" \
		$(if $(EXTRA_VARS),-e "$(EXTRA_VARS)",) \
		--vault-password-file "$(VAULT_PASS_FILE)"

infra-ansible-dry-run: infra-known-hosts infra-check-playbook
	@echo -e "$(BLUE)▶ Ansible dry-run (workspace: $(TF_WORKSPACE), limit: $(ANSIBLE_LIMIT))$(RESET)"
	@$(ensure_vault)
	@cd "$(ANSIBLE_DIR)" && \
	$(ANSIBLE_PLAYBOOK_BIN) -i "$(ANSIBLE_INVENTORY)" "$(ANSIBLE_PLAYBOOK)" \
		--check --diff \
		--limit "$(ANSIBLE_LIMIT)" \
		$(if $(EXTRA_VARS),-e "$(EXTRA_VARS)",) \
		--vault-password-file "$(VAULT_PASS_FILE)"

infra-ansible-deploy: infra-known-hosts infra-check-playbook
	@echo -e "$(BLUE)▶ Ansible deploy (tags=deploy) (workspace: $(TF_WORKSPACE), limit: $(ANSIBLE_LIMIT))$(RESET)"
	@$(ensure_vault)
	@cd "$(ANSIBLE_DIR)" && \
	$(ANSIBLE_PLAYBOOK_BIN) -i "$(ANSIBLE_INVENTORY)" "$(ANSIBLE_PLAYBOOK)" \
		--tags deploy \
		--limit "$(ANSIBLE_LIMIT)" \
		$(if $(EXTRA_VARS),-e "$(EXTRA_VARS)",) \
		--vault-password-file "$(VAULT_PASS_FILE)"

# One-command deploy
.PHONY: deploy deploy-blue deploy-green

deploy: infra-init infra-apply infra-inventory infra-known-hosts infra-ansible-playbook
	@echo -e "$(GREEN)Deploy done: workspace=$(TF_WORKSPACE)$(RESET)"

deploy-blue:
	@$(MAKE) deploy ENV=prod STACK=blue

deploy-green:
	@$(MAKE) deploy ENV=prod STACK=green

# Debug / SSH
.PHONY: show-config infra-ssh

show-config:
	@echo -e "$(BOLD)ENV$(RESET)=$(ENV)"
	@echo -e "$(BOLD)STACK$(RESET)=$(STACK)"
	@echo -e "$(BOLD)TF_WORKSPACE$(RESET)=$(TF_WORKSPACE)"
	@echo -e "$(BOLD)TFVARS$(RESET)=$(TFVARS)"
	@echo -e "$(BOLD)TFVARS_PATH$(RESET)=$(TFVARS_PATH)"
	@echo -e "$(BOLD)ANSIBLE_DIR$(RESET)=$(ANSIBLE_DIR)"
	@echo -e "$(BOLD)ANSIBLE_PLAYBOOK$(RESET)=$(ANSIBLE_PLAYBOOK)"
	@echo -e "$(BOLD)ANSIBLE_INVENTORY$(RESET)=$(ANSIBLE_INVENTORY)"
	@echo -e "$(BOLD)ANSIBLE_INVENTORY_PATH$(RESET)=$(ANSIBLE_INVENTORY_PATH)"
	@echo -e "$(BOLD)ANSIBLE_LIMIT$(RESET)=$(ANSIBLE_LIMIT)"
	@echo -e "$(BOLD)VAULT_PASS_FILE$(RESET)=$(VAULT_PASS_FILE)"
	@echo -e "$(BOLD)INFRA_SSH_USER$(RESET)=$(INFRA_SSH_USER)"
	@echo -e "$(BOLD)INFRA_SSH_HOST$(RESET)=$(INFRA_SSH_HOST)"

infra-ssh:
	@test -n "$(INFRA_SSH_HOST)" || (echo -e "$(RED)Set INFRA_SSH_HOST=<ip>$(RESET)"; exit 1)
	@echo -e "$(BLUE)▶ SSH:$(RESET) $(INFRA_SSH_USER)@$(INFRA_SSH_HOST)"
	@ssh "$(INFRA_SSH_USER)@$(INFRA_SSH_HOST)"
