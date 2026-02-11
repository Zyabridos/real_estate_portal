SHELL := /bin/bash

K3D_CLUSTER       ?= realestate
K8S_NS            ?= realestate
K8S_DIR           ?= k8s
K8S_BASE          ?= $(K8S_DIR)/base
K8S_OVERLAYS      ?= $(K8S_DIR)/overlays

# Environment: dev | prod
K8S_ENV           ?= dev
K8S_KUSTOMIZE_DIR ?= $(K8S_OVERLAYS)/$(K8S_ENV)

# Setup default
K8S_LOG_TAIL      ?= 200
K8S_TIMEOUT       ?= 300

# Ingress / hosts (smoke)
DEV_FRONT_HOST    ?= localhost
DEV_CMS_HOST      ?= cms.localhost

PROD_FRONT_HOST   ?= www.realestateproject.casa
PROD_CMS_HOST     ?= cms.realestateproject.casa

# Ports (for port-forwarding)
API_PORT          ?= 5000
FRONT_PORT        ?= 80
CMS_PORT          ?= 80
MONGO_PORT        ?= 27017

help-k8s:
	@echo -e "$(BOLD)RealEstate Kubernetes commands (k3d + kustomize)$(RESET)"
	@echo -e ""
	@echo -e "$(YELLOW)Config:$(RESET)"
	@echo -e "  $(GREEN)K8S_ENV$(RESET)=$(K8S_ENV)           - overlay to use (dev|prod). Override: $(GREEN)make k8s-apply K8S_ENV=prod$(RESET)"
	@echo -e "  $(GREEN)K3D_CLUSTER$(RESET)=$(K3D_CLUSTER)   - k3d cluster name"
	@echo -e "  $(GREEN)K8S_NS$(RESET)=$(K8S_NS)             - kubernetes namespace"
	@echo -e ""

	@echo -e "$(YELLOW)Cluster:$(RESET)"
	@echo -e "  $(GREEN)make k8s-up$(RESET)                  - create k3d cluster (traefik disabled), set kubectl context + namespace"
	@echo -e "  $(GREEN)make k8s-down$(RESET)                - delete k3d cluster"
	@echo -e "  $(GREEN)make k8s-context$(RESET)             - set kubectl context + namespace"
	@echo -e ""

	@echo -e "$(YELLOW)Kustomize (overlay=$(K8S_ENV)):$(RESET)"
	@echo -e "  $(GREEN)make k8s-build$(RESET)               - render overlay to /tmp/realestate-$(K8S_ENV).yaml"
	@echo -e "  $(GREEN)make k8s-validate$(RESET)            - kubectl apply --dry-run=client"
	@echo -e "  $(GREEN)make k8s-apply$(RESET)               - apply overlay"
	@echo -e "  $(GREEN)make k8s-delete$(RESET)              - delete overlay"
	@echo -e "  $(GREEN)make k8s-reset$(RESET)               - delete + apply + wait + urls (current overlay)"
	@echo -e "  $(GREEN)make k8s-reset-dev$(RESET)           - reset dev overlay"
	@echo -e "  $(GREEN)make k8s-reset-prod$(RESET)          - reset prod overlay"
	@echo -e ""

	@echo -e "$(YELLOW)Logs:$(RESET)"
	@echo -e "  $(GREEN)make k8s-logs-api$(RESET)            - tail API logs"
	@echo -e "  $(GREEN)make k8s-logs-frontend$(RESET)       - tail frontend logs"
	@echo -e "  $(GREEN)make k8s-logs-cms$(RESET)            - tail cms logs"
	@echo -e "  $(GREEN)make k8s-logs-mongo$(RESET)          - tail mongo logs"
	@echo -e "  $(GREEN)make k8s-logs$(RESET)		         - tail all logs above"
	@echo -e ""

	@echo -e "$(YELLOW)Port-forward:$(RESET)"
	@echo -e "  $(GREEN)make k8s-pf-api$(RESET)              - http://localhost:$(API_PORT)/api/health"
	@echo -e "  $(GREEN)make k8s-pf-frontend$(RESET)         - http://localhost:8080"
	@echo -e "  $(GREEN)make k8s-pf-cms$(RESET)              - http://localhost:3333"
	@echo -e "  $(GREEN)make k8s-pf-mongo$(RESET)            - mongodb://localhost:$(MONGO_PORT)"
	@echo -e ""

	@echo -e "$(YELLOW)Smoke:$(RESET)"
	@echo -e "  $(GREEN)make k8s-smoke-dev$(RESET)           - smoke test dev routing (Host headers)"
	@echo -e "  $(GREEN)make k8s-smoke-prod$(RESET)          - smoke test prod routing (Host headers, local)"

k8s-use-ns:
	kubectl config set-context --current --namespace=$(K8S_NS)

# -----------------------------
# Cluster lifecycle (local playground)
k8s-up:
	@echo "$(BLUE)Creating k3d cluster: $(K3D_CLUSTER)$(RESET)\n"
	k3d cluster create $(K3D_CLUSTER) --servers 1 --agents 1 --api-port 6550 \
		-p "80:80@loadbalancer" -p "443:443@loadbalancer" \
		--k3s-arg "--disable=traefik@server:*"
	$(MAKE) k8s-context
	@echo "$(GREEN)Cluster ready$(RESET)\n"

k8s-down:
	@echo "$(YELLOW)Deleting k3d cluster: $(K3D_CLUSTER)$(RESET)\n"
	k3d cluster delete $(K3D_CLUSTER)

# -----
# Build / validate (kustomize)

k8s-build:
	@echo "$(BLUE)Building kustomize overlay: $(K8S_KUSTOMIZE_DIR)$(RESET)\n"
	kubectl kustomize $(K8S_KUSTOMIZE_DIR) > /tmp/realestate-$(K8S_ENV).yaml
	@echo "$(GREEN)OK$(RESET) -> /tmp/realestate-$(K8S_ENV).yaml\n"
	@wc -l /tmp/realestate-$(K8S_ENV).yaml

k8s-validate:
	@echo "$(BLUE)Validating (dry-run=client): $(K8S_KUSTOMIZE_DIR)$(RESET)\n"
	kubectl apply -k $(K8S_KUSTOMIZE_DIR) --dry-run=client

# -----
# Apply / delete / reset

k8s-apply:
	@echo "$(BLUE)Applying overlay: $(K8S_ENV)$(RESET)\n"
	kubectl apply -k $(K8S_KUSTOMIZE_DIR)

k8s-delete:
	@echo "$(BLUE)Deleting overlay: $(K8S_ENV)$(RESET)\n"
	-kubectl delete -k $(K8S_KUSTOMIZE_DIR) --ignore-not-found=true

k8s-reset:
	@echo "$(BLUE)Reset overlay: $(K8S_ENV)$(RESET)\n"
	$(MAKE) k8s-context
	$(MAKE) k8s-delete K8S_ENV=$(K8S_ENV)
	$(MAKE) k8s-apply  K8S_ENV=$(K8S_ENV)
	$(MAKE) k8s-wait-all
	@echo "$(GREEN) Done "

k8s-reset-dev:
	$(MAKE) k8s-reset K8S_ENV=dev

k8s-reset-prod:
	$(MAKE) k8s-reset K8S_ENV=prod

# -----
# Wait (helper used in some make-commands)

k8s-wait-frontend:
	kubectl -n $(K8S_NS) rollout status deploy/realestate-frontend --timeout=$(K8S_TIMEOUT)s

k8s-wait-cms:
	kubectl -n $(K8S_NS) rollout status deploy/realestate-cms --timeout=$(K8S_TIMEOUT)s

k8s-wait-api:
	kubectl -n $(K8S_NS) rollout status deploy/realestate-api --timeout=$(K8S_TIMEOUT)s

k8s-wait-mongo:
	kubectl -n $(K8S_NS) rollout status statefulset/realestate-mongo --timeout=$(K8S_TIMEOUT)s

k8s-wait-all: k8s-wait-mongo k8s-wait-api k8s-wait-frontend k8s-wait-cms

# -----
# Logs
k8s-logs-api:
	kubectl logs -l app=realestate-api -c api --tail=$(K8S_LOG_TAIL) -f

k8s-logs-frontend:
	kubectl logs -l app=realestate-frontend --tail=$(K8S_LOG_TAIL) -f

k8s-logs-cms:
	kubectl logs -l app=realestate-cms --tail=$(K8S_LOG_TAIL) -f

k8s-logs-mongo:
	kubectl logs -l app=realestate-mongo --tail=$(K8S_LOG_TAIL) -f

k8s-logs: k8s-logs-api k8s-logs-frontend k8s-logs-mongo k8s-logs-cms 

# -----
# Port-forward
k8s-pf-api:
	@echo "$(YELLOW)API -> http://localhost:$(API_PORT) (Ctrl+C to stop)$(RESET)\n"
	kubectl port-forward svc/realestate-api-svc $(API_PORT):$(API_PORT)

k8s-pf-frontend:
	@echo "$(YELLOW)Frontend -> http://localhost:8080 (Ctrl+C to stop)$(RESET)\n"
	kubectl port-forward svc/realestate-frontend-svc 8080:80

k8s-pf-cms:
	@echo "$(YELLOW)CMS -> http://localhost:3333 (Ctrl+C to stop)$(RESET)\n"
	kubectl port-forward svc/realestate-cms-svc 3333:80

k8s-pf-mongo:
	@echo "$(YELLOW)Mongo -> localhost:27017 (Ctrl+C to stop)$(RESET)\n"
	kubectl port-forward svc/realestate-mongo $(MONGO_PORT):$(MONGO_PORT)

# -----
# Smoke tests
k8s-smoke-dev:
	@set -e; \
	echo -e "$(YELLOW) DEV SMOKE K8S (ingress) ==$(RESET)"; \
	echo -e "$(BLUE)-- Front (Host: $(DEV_FRONT_HOST)) --$(RESET)"; \
	curl -fsS -I http://localhost/ -H "Host: $(DEV_FRONT_HOST)" | head -n 8 || { echo -e "$(RED)FAILED: Frontend (dev)$(RESET)"; exit 1; }; \
	echo -e "$(BLUE)-- API (Host: $(DEV_FRONT_HOST)) --$(RESET)"; \
	curl -fsS -I http://localhost/api/health -H "Host: $(DEV_FRONT_HOST)" | head -n 8 || { echo -e "$(RED)FAILED: API /api/health (dev)$(RESET)"; exit 1; }; \
	echo -e "$(BLUE)-- CMS (Host: $(DEV_CMS_HOST)) --$(RESET)"; \
	curl -fsS -I http://localhost/ -H "Host: $(DEV_CMS_HOST)" | head -n 8 || { echo -e "$(RED)FAILED: CMS (dev)$(RESET)"; exit 1; }; \
	echo -e "$(GREEN)OK$(RESET)"

k8s-smoke-prod:
	@set -e; \
	echo -e "$(YELLOW) PROD SMOKE K8S (host headers, local) ==$(RESET)"; \
	echo -e "$(BLUE)-- Front (Host: $(PROD_FRONT_HOST)) --$(RESET)"; \
	curl -fsS -I http://localhost/ -H "Host: $(PROD_FRONT_HOST)" | head -n 8 || { echo -e "$(RED)FAILED: Frontend (prod host)$(RESET)"; exit 1; }; \
	echo -e "$(BLUE)-- API (Host: $(PROD_FRONT_HOST)) --$(RESET)"; \
	curl -fsS -I http://localhost/api/health -H "Host: $(PROD_FRONT_HOST)" | head -n 8 || { echo -e "$(RED)FAILED: API /api/health (prod host)$(RESET)"; exit 1; }; \
	echo -e "$(BLUE)-- CMS (Host: $(PROD_CMS_HOST)) --$(RESET)"; \
	curl -fsS -I http://localhost/ -H "Host: $(PROD_CMS_HOST)" | head -n 8 || { echo -e "$(RED)FAILED: CMS (prod host)$(RESET)"; exit 1; }; \
	echo -e "$(GREEN)OK$(RESET)"
