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

# Local development
dev-frontend:
	cd frontend && npm run dev

dev-backend:
	cd backend/src/RealEstate.Api && dotnet watch run

dev-cms:
	cd cms && npm run dev

dev:
	make dev-backend && make dev-frontend && make dev-cms

backend-full-rebuild:
	cd backend && \
    dotnet nuget locals all --clear && \
    rm -rf src/**/bin src/**/obj && \
    dotnet restore --disable-parallel && \
    dotnet build && \
    dotnet run --project src/RealEstate.Api/RealEstate.Api.csproj

frontend-clean-install:
	cd frontend && \
	rm -rf node_modules package-lock.json && \
	npm install

test-back:
	@echo "$(LIGHT_BLUE)Starting tests for backend...$(RESET)"
	cd backend && dotnet test RealEstate.slnx

# Pings
ping-api:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/ping_api.sh

ping-properties:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/ping_properties.sh

ping-property:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/ping_property_by_id.sh "$(ID)"

ping-brokers:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/ping_brokers.sh

ping-broker:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/ping_broker_by_id.sh "$(ID)"

ping-leads:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/ping_leads.sh

ping-lead:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/ping_lead_by_id.sh "$(ID)"

smoke-api: ping-api ping-properties ping-brokers ping-leads

# Seeds
seed-brokers:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/seed/seed_brokers.sh

seed-properties:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/seed/seed_properties.sh

seed-leads:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/seed/seed_leads.sh

seed: seed-brokers seed-properties seed-leads

# Docker
build:
	@echo "$(LIGHT_BLUE)Building Docker images...$(RESET)"
	docker compose build

up:
	@echo "$(LIGHT_BLUE)Starting all services...$(RESET)"
	docker compose up

up-d:
	@echo "$(LIGHT_BLUE)Starting all services in background...$(RESET)"
	docker compose up -d

down:
	@echo "$(YELLOW)Stopping and removing all containers...$(RESET)"
	docker compose down

restart:
	@echo "$(YELLOW)Restarting core Docker services (no cms)...$(RESET)"
	docker compose restart $(CORE_SERVICES)

restart-frontend:
	@echo "$(YELLOW)Restarting frontend...$(RESET)"
	docker compose restart frontend

restart-backend:
	@echo "$(YELLOW)Restarting backend...$(RESET)"
	docker compose restart backend

restart-db:
	@echo "$(YELLOW)Restarting mongodb...$(RESET)"
	docker compose restart mongodb

restart-with-cms:
	@echo "$(YELLOW)Restarting core services + cms...$(RESET)"
	docker compose restart $(CORE_SERVICES) $(CMS_SERVICE)

rebuild:
	@echo "$(PURPLE)Rebuilding Docker services...$(RESET)"
	docker compose down
	docker compose build
	docker compose up -d

# Docker Shell
sh-backend:
	@echo "$(GREEN)Opening shell in backend...$(RESET)"
	docker compose exec $(BACKEND) sh

sh-frontend:
	@echo "$(GREEN)Opening shell in frontend...$(RESET)"
	docker compose exec $(FRONTEND) sh

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
	@echo "  $(GREEN)ping-api$(RESET)              - Check backend health endpoint"
	@echo "  $(GREEN)ping-properties$(RESET)       - Fetch properties list"
	@echo "  $(GREEN)ping-property ID=<guid>$(RESET)- Fetch property by id"
	@echo "  $(GREEN)smoke-api$(RESET)             - Run basic API smoke checks"
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

	@echo "$(PURPLE)Usage:$(RESET)"
	@echo "  make <command>"
	@echo ""
