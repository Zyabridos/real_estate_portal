include make/dev.mk
include make/pings.mk
include make/seeds.mk
include make/docker.mk
include make/infrastructure.mk

# Project services
BACKEND   		= backend
FRONTEND  		= frontend
CORE_SERVICES   = mongodb backend frontend
CMS_SERVICE     = cms

# Ports and URLs (single source of truth is scripts/lib/config.sh,
# but we pass defaults from Makefile too if you want)
BACKEND_PORT    ?= 5055
FRONTEND_PORT   ?= 3000
MONGO_PORT      ?= 27017
BACKEND_URL     ?= http://localhost:$(BACKEND_PORT)

# ANSI colors & styles
RESET    = \033[0m
BOLD     = \033[1m

RED			= \033[1;31m
GREEN		= \033[1;32m
YELLOW		= \033[1;33m
BLUE		= \033[1;34m
PURPLE		= \033[1;35m
LIGHT_BLUE  = \033[1;36m

# Help
help:
	@echo ""
	@echo "$(LIGHT_BLUE)==================================================$(RESET)"
	@echo "$(GREEN)      Real Estate Project — Available Commands$(RESET)"
	@echo "$(LIGHT_BLUE)==================================================$(RESET)"
	@echo ""

	@echo "$(YELLOW)Local development:$(RESET)"
	@echo "  $(GREEN)dev-backend$(RESET)            - Run backend locally (dotnet watch)"
	@echo "  $(GREEN)dev-frontend$(RESET)           - Run frontend locally (Vite)"
	@echo "  $(GREEN)dev-cms$(RESET)                - Run Sanity Studio locally"
	@echo "  $(GREEN)dev$(RESET)                    - Run backend + frontend + cms"
	@echo ""

	@echo "$(YELLOW)API / Ports:$(RESET)"
	@echo "  $(GREEN)ping-api$(RESET)                              - Check backend health endpoint"
	@echo "  $(GREEN)ping-entities ENTITY=<name>$(RESET)           - Fetch list (properties | brokers | leads)"
	@echo "  $(GREEN)ping-entity ENTITY=<name> ID=<guid>$(RESET)   - Fetch by id"
	@echo "  $(GREEN)smoke-api$(RESET)                             - Run basic API smoke checks"
	@echo "  $(GREEN)Aliases:$(RESET) ping-properties/ping-property, ping-brokers/ping-broker, ping-leads/ping-lead"
	@echo ""

	@echo "$(YELLOW)Reset / bootstrap (local):$(RESET)"
	@echo "  $(GREEN)backend-full-rebuild$(RESET)   - Full backend reset (NuGet, bin/obj, rebuild & run)"
	@echo "  $(GREEN)frontend-clean-install$(RESET) - Full frontend reset (node_modules reinstall)"
	@echo ""

	@echo "$(YELLOW)Testing:$(RESET)"
	@echo "  $(GREEN)test-back$(RESET)              - Run backend test suite"
	@echo ""

	@echo "$(YELLOW)Docker commands:$(RESET)"
	@echo "  $(GREEN)build$(RESET)                  - Build Docker images"
	@echo "  $(GREEN)up$(RESET)                     - Start all services (foreground)"
	@echo "  $(GREEN)up-d$(RESET)                   - Start all services (background)"
	@echo "  $(GREEN)down$(RESET)                   - Stop and remove all containers"
	@echo "  $(GREEN)rebuild$(RESET)                - Stop, rebuild and restart all services"
	@echo ""

	@echo "$(YELLOW)Docker service management:$(RESET)"
	@echo "  $(GREEN)restart$(RESET)                - Restart core services ($(CORE_SERVICES))"
	@echo "  $(GREEN)restart-backend$(RESET)        - Restart backend container"
	@echo "  $(GREEN)restart-frontend$(RESET)       - Restart frontend container"
	@echo "  $(GREEN)restart-db$(RESET)             - Restart mongodb container"
	@echo "  $(GREEN)restart-with-cms$(RESET)       - Restart core services + cms"
	@echo ""

	@echo "$(YELLOW)Shell inside containers:$(RESET)"
	@echo "  $(GREEN)sh-backend$(RESET)             - Shell into backend container"
	@echo "  $(GREEN)sh-frontend$(RESET)            - Shell into frontend container"
	@echo ""

	@echo "$(YELLOW)Cleanup:$(RESET)"
	@echo "  $(GREEN)prune$(RESET)                  - Remove unused Docker resources"
	@echo "  $(GREEN)clean$(RESET)                  - Full Docker cleanup ($(RED)danger!$(RESET))"
	@echo ""

	@echo "$(YELLOW)Infrastructure (Terraform + Ansible):$(RESET)"
	@echo "  $(GREEN)infra-init$(RESET)             - Terraform init"
	@echo "  $(GREEN)infra-plan$(RESET)             - Terraform plan (workspace: $(TF_WORKSPACE))"
	@echo "  $(GREEN)infra-apply$(RESET)            - Terraform apply"
	@echo "  $(GREEN)infra-output$(RESET)           - Terraform outputs"
	@echo "  $(GREEN)infra-fmt$(RESET)              - Terraform fmt"
	@echo "  $(GREEN)infra-validate$(RESET)         - Terraform validate"
	@echo "  $(GREEN)infra-destroy$(RESET)          - Terraform destroy ($(RED)danger!$(RESET))"
	@echo ""
	@echo "  $(GREEN)infra-ansible-ping$(RESET)     - Ansible ping (limit: $(ANSIBLE_LIMIT))"
	@echo "  $(GREEN)infra-ansible-dry-run$(RESET)  - Ansible playbook --check --diff"
	@echo "  $(GREEN)infra-ansible-playbook$(RESET) - Run full playbook"
	@echo "  $(GREEN)infra-ansible-deploy$(RESET)   - Run deploy tasks (--tags deploy)"
	@echo "  $(GREEN)infra-ssh$(RESET)              - SSH helper (INFRA_SSH_HOST=...)"
	@echo ""

	@echo "$(PURPLE)Usage:$(RESET)"
	@echo "  make <command>"
	@echo ""