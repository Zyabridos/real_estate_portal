tf-init:
	@echo -e "$(BLUE)Terraform init ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) init

tf-format:
	@echo -e "$(BLUE)Terraform formatting (pretty-print) ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) fmt -recursive

tf-validate:
	@echo -e "$(BLUE)Terraform validate ($(TERRAFORM_DIR))$(RESET)"
	@$(TF) -chdir=$(TERRAFORM_DIR) validate

tf-plan:
	@echo -e "$(BLUE)Terraform plan (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) plan -var-file="$(TFVARS_PATH)"

tf-apply:
	@echo -e "$(BLUE)Terraform apply (workspace: $(TF_WORKSPACE), tfvars: $(TFVARS_PATH))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@echo -e "  $(YELLOW)workspace:$(RESET) $$($(TF) -chdir=$(TERRAFORM_DIR) workspace show)"
	@$(TF) -chdir=$(TERRAFORM_DIR) apply -auto-approve -var-file="$(TFVARS_PATH)"

tf-output:
	@echo -e "$(BLUE)Terraform output (workspace: $(TF_WORKSPACE))$(RESET)"
	@$(TF_SELECT_WORKSPACE)
	@$(TF) -chdir=$(TERRAFORM_DIR) output

# Ansible
.PHONY: ansible-inventory ansible-known-hosts ansible-ping ansible-playbook ansible-show-diff
ansible-known-hosts: ansible-inventory
	@echo -e "$(BLUE)Update SSH known_hosts from inventory ($(ANSIBLE_INVENTORY_PATH))$(RESET)"
	@mkdir -p "$(HOME)/.ssh"
	@chmod 700 "$(HOME)/.ssh"
	@touch "$(KNOWN_HOSTS_FILE)"
	@chmod 600 "$(KNOWN_HOSTS_FILE)"
	@{ grep -Eo '([0-9]{1,3}\.){3}[0-9]{1,3}' "$(ANSIBLE_INVENTORY_PATH)" || true; } | sort -u | \
	while read -r ip; do \
		[ -z "$$ip" ] && continue; \
		echo -e "  $(YELLOW)- refreshing host key for$(RESET) $$ip"; \
		$(SSH_KEYGEN) -R "$$ip" >/dev/null 2>&1 || true; \
		$(SSH_KEYSCAN) -H -t ed25519,rsa "$$ip" 2>/dev/null >> "$(KNOWN_HOSTS_FILE)" || true; \
	done

ansible-ping: ansible-known-hosts
	@echo -e "$(BLUE)▶ Ansible ping (limit: $(ANSIBLE_LIMIT))$(RESET)"
	@ANSIBLE_CONFIG="$(ANSIBLE_CONFIG_FILE)" \
	$(ANSIBLE) -i "$(ANSIBLE_INVENTORY_PATH)" all -m ping -l "$(ANSIBLE_LIMIT)"

ansible-playbook: ansible-known-hosts
	@echo -e "$(BLUE)▶ Ansible playbook (workspace: $(TF_WORKSPACE), limit: $(ANSIBLE_LIMIT))$(RESET)"
	@$(call require_vault_pass)
	@ANSIBLE_CONFIG="$(ANSIBLE_CONFIG_FILE)" \
	ansible-playbook -i "$(ANSIBLE_INVENTORY_PATH)" "$(ANSIBLE_DIR)/$(ANSIBLE_MAIN_PLAYBOOK)" \
		--limit "$(ANSIBLE_LIMIT)" \
		-e "$(ANSIBLE_BASE_VARS) $(EXTRA_VARS)" \
		--vault-password-file "$(ANSIBLE_DIR)/$(VAULT_PASS_FILE)"

ansible-show-diff: ansible-known-hosts
	@echo -e "$(BLUE)▶ Ansible dry-run (workspace: $(TF_WORKSPACE), limit: $(ANSIBLE_LIMIT))$(RESET)"
	@$(call require_vault_pass)
	@ANSIBLE_CONFIG="$(ANSIBLE_CONFIG_FILE)" \
	ansible-playbook -i "$(ANSIBLE_INVENTORY_PATH)" "$(ANSIBLE_DIR)/$(ANSIBLE_MAIN_PLAYBOOK)" \
		--check --diff \
		--limit "$(ANSIBLE_LIMIT)" \
		-e "$(ANSIBLE_BASE_VARS) $(EXTRA_VARS)" \
		--vault-password-file "$(ANSIBLE_DIR)/$(VAULT_PASS_FILE)"

# Deploy app (ansible only)
deploy-app: ansible-known-hosts ansible-playbook
	@echo -e "$(GREEN)App deploy done: workspace=$(TF_WORKSPACE)$(RESET)"

deploy-images-blue:
	@$(MAKE) deploy-app ENV=prod STACK=blue \
	  EXTRA_VARS="env=prod stack_id=blue api_image_tag=$(IMAGE_TAG) frontend_image_tag=$(IMAGE_TAG) cms_image_tag=$(IMAGE_TAG)"

deploy-images-green:
	@$(MAKE) deploy-app ENV=prod STACK=green \
	  EXTRA_VARS="env=prod stack_id=green api_image_tag=$(IMAGE_TAG) frontend_image_tag=$(IMAGE_TAG) cms_image_tag=$(IMAGE_TAG)"

release-blue: push-images deploy-images-blue
	@echo "$(GREEN)Release BLUE done: tag=$(IMAGE_TAG)$(RESET)"

release-green: push-images deploy-images-green
	@echo "$(GREEN)Release GREEN done: tag=$(IMAGE_TAG)$(RESET)"

# One-command deploy (terraform + ansible)
deploy: tf-init tf-apply release-blue release-green
	@echo -e "$(GREEN)Deploy done: workspace=$(TF_WORKSPACE)$(RESET)"

# Debug / SSH
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
	@echo -e "$(BLUE)SSH:$(RESET) $(INFRA_SSH_USER)@$(INFRA_SSH_HOST)"
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
