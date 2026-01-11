# Project services
BACKEND   		= backend
FRONTEND  		= frontend
DATABASE		= mongo
CORE_SERVICES   = mongodb backend frontend
CMS_SERVICE     = cms

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
	cd backend/RealEstate.Api && dotnet watch run

dev-cms:
	cd cms && npm run dev

dev:
	make dev-backend && make dev-frontend && make dev-cms

test-back:
	@echo "$(LIGHT_BLUE)Starting integration tests for backend...$(RESET)"
	cd backend && dotnet test RealEstate.slnx

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
	docker compose up

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
	@echo "$(LIGHT_BLUE)==============================================$(RESET)"
	@echo "$(GREEN)      Real Estate Project — Docker Commands$(RESET)"
	@echo "$(LIGHT_BLUE)==============================================$(RESET)"
	@echo ""
	@echo "$(YELLOW)Main Commands:$(RESET)"
	@echo "  $(GREEN)build$(RESET)              - Build Docker images"
	@echo "  $(GREEN)up$(RESET)                 - Start all services (foreground)"
	@echo "  $(GREEN)up-d$(RESET)               - Start all services in background"
	@echo "  $(GREEN)down$(RESET)               - Stop and remove all containers"
	@echo "  $(GREEN)rebuild$(RESET)            - Stop, rebuild and restart all services"
	@echo "  $(GREEN)restart$(RESET)            - Restart core services (mongodb/backend/frontend)"
	@echo "  $(GREEN)restart-frontend$(RESET)   - Restart only frontend"
	@echo "  $(GREEN)restart-backend$(RESET)    - Restart only backend"
	@echo "  $(GREEN)restart-db$(RESET)         - Restart only mongodb"
	@echo "  $(GREEN)restart-with-cms$(RESET)   - Restart core services + cms"
	@echo ""
	@echo "$(YELLOW)Shell inside containers:$(RESET)"
	@echo "  $(GREEN)sh-backend$(RESET)         - Shell into backend container"
	@echo "  $(GREEN)sh-frontend$(RESET)        - Shell into frontend container"
	@echo ""
	@echo " $(YELLOW)Cleanup:$(RESET)"
	@echo "  $(GREEN)prune$(RESET)              - Remove Docker unused resources"
	@echo "  $(GREEN)clean$(RESET)              - Full cleanup ($(RED)danger!$(RESET))"
	@echo ""
	@echo "$(PURPLE)Usage:$(RESET)"
	@echo "  make <command>"
	@echo ""
