ENV_FILE ?= .env.development

FRONTEND_IMAGE ?= zyabridos/real_estate_prod_frontend:latest
BACKEND_IMAGE ?= zyabridos/real_estate_prod_backend:latest
CMS_IMAGE ?= zyabridos/real_estate_prod_cms:latest

DOCKERFILE_PATH_FRONTEND = ./frontend/Dockerfile.production
DOCKERFILE_PATH_BACKEND = ./backend/Dockerfile.production
DOCKERFILE_PATH_CMS = ./cms/Dockerfile 

# Requires variables: CORE_SERVICES, CMS_SERVICE, BACKEND, FRONTEND
COMPOSE = docker compose --env-file $(ENV_FILE)

build:
	@echo "$(LIGHT_BLUE)Building Docker images...$(RESET)"
	$(COMPOSE) build

up:
	@echo "$(LIGHT_BLUE)Starting all services...$(RESET)"
	$(COMPOSE) up

up-d:
	@echo "$(LIGHT_BLUE)Starting all services in background...$(RESET)"
	$(COMPOSE) up -d

down:
	@echo "$(YELLOW)Stopping and removing all containers...$(RESET)"
	$(COMPOSE) down

down-v:
	@echo "$(YELLOW)Stopping and removing all containers + volumes...$(RESET)"
	$(COMPOSE) down -v

restart:
	@echo "$(YELLOW)Restarting core Docker services (no cms)...$(RESET)"
	$(COMPOSE) restart $(CORE_SERVICES)

restart-frontend:
	@echo "$(YELLOW)Restarting frontend...$(RESET)"
	$(COMPOSE) restart frontend

restart-backend:
	@echo "$(YELLOW)Restarting backend...$(RESET)"
	$(COMPOSE) restart backend

restart-db:
	@echo "$(YELLOW)Restarting mongodb...$(RESET)"
	$(COMPOSE) restart mongodb

restart-with-cms:
	@echo "$(YELLOW)Restarting core services + cms...$(RESET)"
	$(COMPOSE) restart $(CORE_SERVICES) $(CMS_SERVICE)

rebuild:
	@echo "$(PURPLE)Rebuilding Docker services...$(RESET)"
	$(COMPOSE) down
	$(COMPOSE) build
	$(COMPOSE) up -d

# Rebuild and restart single services
rebuild-frontend:
	@echo "$(PURPLE)Rebuilding frontend...$(RESET)"
	$(COMPOSE) up -d --build --force-recreate $(FRONTEND)

rebuild-backend:
	@echo "$(PURPLE)Rebuilding backend...$(RESET)"
	$(COMPOSE) up -d --build --force-recreate $(BACKEND)

rebuild-cms:
	@echo "$(PURPLE)Rebuilding cms...$(RESET)"
	$(COMPOSE) up -d --build --force-recreate $(CMS_SERVICE)

rebuild-db:
	@echo "$(PURPLE)Rebuilding mongodb...$(RESET)"
	$(COMPOSE) up -d --build --force-recreate mongodb

# Docker Shell
sh-backend:
	@echo "$(GREEN)Opening shell in backend...$(RESET)"
	$(COMPOSE) exec $(BACKEND) sh

sh-frontend:
	@echo "$(GREEN)Opening shell in frontend...$(RESET)"
	$(COMPOSE) exec $(FRONTEND) sh

# Build and push to Docker Hub Single Services:
push-frontend:
	@echo "$(PURPLE)Building frontend image...$(RESET)"
	docker build -f $(DOCKERFILE_PATH_FRONTEND) -t $(FRONTEND_IMAGE) ./frontend
	@echo "$(PURPLE)Pushing frontend image to Docker Hub...$(RESET)"
	docker push $(FRONTEND_IMAGE)
	
push-backend:
	docker build -f $(DOCKERFILE_PATH_BACKEND) -t $(BACKEND_IMAGE) ./backend && \
	@echo "$(PURPLE)Pushing backend image to Docker Hub...$(RESET)"
	docker push $(BACKEND_IMAGE)

push-cmsd:
	docker build -f $(DOCKERFILE_PATH_CMS)e -t $(CMS_IMAGE) ./cms && \
	@echo "$(PURPLE)Pushing CMS image to Docker Hub...$(RESET)"
	docker push $(CMS_IMAGE)