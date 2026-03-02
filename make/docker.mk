build:
	@echo "$(LIGHT_BLUE)Building Docker images...$(RESET)"
	$(COMPOSE) build $(CORE_SERVICES)

up:
	@echo "$(LIGHT_BLUE)Starting services: $(CORE_SERVICES)$(RESET)"
	$(COMPOSE) up $(CORE_SERVICES)

up-d:
	@echo "$(LIGHT_BLUE)Starting $(CORE_SERVICES) services in background...$(RESET)"
	$(COMPOSE) up -d $(CORE_SERVICES)

up-with-cms:
	@echo "$(LIGHT_BLUE)Starting all services...$(RESET)"
	$(COMPOSE) up $(CORE_SERVICES) $(SERVICE_CMS)

up-with-cms-d:
	@echo "$(LIGHT_BLUE)Starting all services in background...$(RESET)"
	$(COMPOSE) up -d $(CORE_SERVICES) $(SERVICE_CMS)

down:
	@echo "$(YELLOW)Stopping and removing all containers...$(RESET)"
	$(COMPOSE) down

down-v:
	@echo "$(YELLOW)Stopping and removing all containers + volumes...$(RESET)"
	$(COMPOSE) down -v

clean:
	@echo "$(RED) This will remove ALL Docker data: containers, images, volumes, cache.$(RESET)"
	@echo -n "Type 'yes' to continue. Only yes will be accepted" && read ans && \
	"$$ans" = "yes" || \
	( echo "Cancelled."; exit 1 )
	@echo "Cleaning Docker system..."
	docker system prune -a --volumes -f

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

# Build and push to Docker Hub:

push-frontend:
	@echo "$(PURPLE)Building frontend image: $(FRONTEND_IMAGE)$(RESET)"
	@test -f "$(FRONTEND_ENV_FILE)" || (echo "$(RED)Missing $(FRONTEND_ENV_FILE). Create it or override FRONTEND_ENV_FILE=...$(RESET)"; exit 1)
	@set -e; \
	set -a; . "$(FRONTEND_ENV_FILE)"; set +a; \
	docker build -f frontend/Dockerfile.production \
	  --build-arg VITE_API_BASE_URL="/api" \
	  --build-arg VITE_SANITY_PROJECT_ID="$$VITE_SANITY_PROJECT_ID" \
	  --build-arg VITE_SANITY_DATASET="$$VITE_SANITY_DATASET" \
	  --build-arg VITE_SANITY_API_VERSION="$$VITE_SANITY_API_VERSION" \
	  --build-arg VITE_SANITY_USE_CDN="$$VITE_SANITY_USE_CDN" \
	  -t "$(FRONTEND_IMAGE)" frontend
	@echo "$(PURPLE)Pushing frontend image...$(RESET)"
	docker push "$(FRONTEND_IMAGE)"
	
push-backend:
	@echo "$(PURPLE)Building backend image: $(BACKEND_IMAGE)$(RESET)"
	docker build -f backend/Dockerfile -t "$(BACKEND_IMAGE)" backend
	@echo "$(PURPLE)Pushing backend image...$(RESET)"
	docker push "$(BACKEND_IMAGE)"

push-cms:
	@echo "$(PURPLE)Building cms image: $(CMS_IMAGE)$(RESET)"
	docker build -f cms/Dockerfile -t "$(CMS_IMAGE)" cms
	@echo "$(PURPLE)Pushing cms image...$(RESET)"
	docker push "$(CMS_IMAGE)"

push-images: push-backend push-frontend push-cms
	@echo "$(GREEN)All images pushed with tag $(IMAGE_TAG)$(RESET)"