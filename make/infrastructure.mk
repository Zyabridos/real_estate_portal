INFRA_DIR        ?= infrastructure

TERRAFORM_DIR    ?= $(INFRA_DIR)/terraform
TF               ?= terraform
TF_WORKSPACE     ?= prod

ANSIBLE_DIR      ?= $(INFRA_DIR)/ansible
ANSIBLE          ?= ansible
ANSIBLE_PLAYBOOK ?= playbook.yml
ANSIBLE_INVENTORY?= inventory.ini
ANSIBLE_LIMIT    ?= all

VAULT_PASS_FILE  ?= vault-password

# SSH helper
INFRA_SSH_USER   ?= root
INFRA_SSH_HOST   ?=

infra-init:
	@echo "$(LIGHT_BLUE)▶ Terraform init$(RESET)"
	@cd $(TERRAFORM_DIR) && $(TF) init

infra-fmt:
	@echo "$(LIGHT_BLUE)▶ Terraform fmt$(RESET)"
	@cd $(TERRAFORM_DIR) && $(TF) fmt -recursive

infra-validate:
	@echo "$(LIGHT_BLUE)▶ Terraform validate$(RESET)"
	@cd $(TERRAFORM_DIR) && $(TF) validate

infra-plan:
	@echo "$(LIGHT_BLUE)▶ Terraform plan$(RESET)"
	@cd $(TERRAFORM_DIR) && \
		$(TF) workspace select $(TF_WORKSPACE) 2>/dev/null || $(TF) workspace new $(TF_WORKSPACE) && \
		$(TF) plan $(if $(TFVARS),-var-file=$(TFVARS),)

infra-apply:
	@echo "$(LIGHT_BLUE)▶ Terraform apply$(RESET)"
	@cd $(TERRAFORM_DIR) && \
		$(TF) workspace select $(TF_WORKSPACE) 2>/dev/null || $(TF) workspace new $(TF_WORKSPACE) && \
		$(TF) apply -auto-approve $(if $(TFVARS),-var-file=$(TFVARS),)

infra-destroy:
	@echo "$(RED)▶ Terraform destroy$(RESET)"
	@cd $(TERRAFORM_DIR) && \
		$(TF) workspace select $(TF_WORKSPACE) 2>/dev/null || $(TF) workspace new $(TF_WORKSPACE) && \
		$(TF) destroy -auto-approve $(if $(TFVARS),-var-file=$(TFVARS),)

infra-output:
	@echo "$(LIGHT_BLUE)▶ Terraform output$(RESET)"
	@cd $(TERRAFORM_DIR) && $(TF) output




infra-ansible-ping:
	@echo "$(LIGHT_BLUE)▶ Ansible ping (limit: $(ANSIBLE_LIMIT))$(RESET)"
	@cd $(ANSIBLE_DIR) && \
		$(ANSIBLE) -i $(ANSIBLE_INVENTORY) $(ANSIBLE_LIMIT) -m ping \
		--vault-password-file $(VAULT_PASS_FILE)

infra-ansible-dry-run:
	@echo "$(LIGHT_BLUE)▶ Ansible dry-run (CHECK + DIFF)$(RESET)"
	@cd $(ANSIBLE_DIR) && \
		ansible-playbook -i $(ANSIBLE_INVENTORY) $(ANSIBLE_PLAYBOOK) \
		--check --diff --limit $(ANSIBLE_LIMIT) \
		--vault-password-file $(VAULT_PASS_FILE)

infra-ansible-playbook:
	@echo "$(LIGHT_BLUE)▶ Ansible playbook$(RESET)"
	@cd $(ANSIBLE_DIR) && \
		ansible-playbook -i $(ANSIBLE_INVENTORY) $(ANSIBLE_PLAYBOOK) \
		--limit $(ANSIBLE_LIMIT) \
		--vault-password-file $(VAULT_PASS_FILE)

infra-ansible-deploy:
	@echo "$(GREEN)▶ Ansible deploy (--tags deploy)$(RESET)"
	@cd $(ANSIBLE_DIR) && \
		ansible-playbook -i $(ANSIBLE_INVENTORY) $(ANSIBLE_PLAYBOOK) \
		--limit $(ANSIBLE_LIMIT) --tags deploy \
		--vault-password-file $(VAULT_PASS_FILE)

infra-ssh:
	@if [ -z "$(INFRA_SSH_HOST)" ]; then \
		echo "$(RED)✖ INFRA_SSH_HOST is not set$(RESET)"; \
		echo "  Example: make infra-ssh INFRA_SSH_HOST=1.2.3.4 INFRA_SSH_USER=root"; \
		exit 1; \
	fi
	@echo "$(LIGHT_BLUE)▶ SSH $(INFRA_SSH_USER)@$(INFRA_SSH_HOST)$(RESET)"
	@ssh $(INFRA_SSH_USER)@$(INFRA_SSH_HOST)
