# Requires variables: CORE_SERVICES, CMS_SERVICE, BACKEND, FRONTEND

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
