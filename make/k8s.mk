K3D_CLUSTER 	  ?= realestate
K8S_NS            ?= realestate
K8S_NAMESPACE     ?= k8s/namespace.yaml

K8S_MONGO_DEPLOY ?= k8s/mongo/deployment.yaml
K8S_MONGO_SVC    ?= k8s/mongo/service.yaml

K8S_API_DEPLOY   ?= k8s/api/deployment.yaml
K8S_API_SVC      ?= k8s/api/service.yaml

K8S_CMS_DEPLOY   ?= k8s/cms/deployment.yaml
K8S_CMS_SVC      ?= k8s/cms/service.yaml

K8S_FRONT_DEPLOY ?= k8s/frontend/deployment.yaml
K8S_FRONT_SVC    ?= k8s/frontend/service.yaml

K8S_INGRESS      ?= k8s/ingress/ingress.yaml

K8S_LOG_TAIL 	 ?= 200
K8S_TIMEOUT		 ?= 180

# Colors (fallback if not defined in root Makefile)
RESET ?= \033[0m
BOLD  ?= \033[1m
GREEN ?= \033[1;32m
YELLOW?= \033[1;33m
RED   ?= \033[1;31m
BLUE  ?= \033[1;34m

# Nodes
k8s-down:
	k3d cluster delete $(K3D_CLUSTER)

k8s-up:
	k3d cluster create $(K3D_CLUSTER) --servers 1 --agents 1 --api-port 6550 \
		-p "80:80@loadbalancer" -p "443:443@loadbalancer" \
		-p "3000:3000@loadbalancer" -p "5001:5001@loadbalancer" -p "3333:3333@loadbalancer"
	$(MAKE) k8s-use-context

# Traefik (k3s default) disable helper (I had some problem when played with NGINX Ingress Controller).
# Needed so localhost:80 routes to ingress-nginx instead of traefik
k8s-disable-traefik:
	kubectl -n kube-system delete helmchart traefik traefik-crd --ignore-not-found
	kubectl -n kube-system delete deploy traefik --ignore-not-found
	kubectl -n kube-system delete svc traefik --ignore-not-found

# Apply/delete
k8s-apply-namespace:
	kubectl apply -f $(K8S_NAMESPACE)

K8S_APP_MANIFESTS := \
	$(K8S_MONGO_DEPLOY) \
	$(K8S_MONGO_SVC) \
	$(K8S_API_DEPLOY) \
	$(K8S_API_SVC) \
	$(K8S_CMS_DEPLOY) \
	$(K8S_CMS_SVC) \
	$(K8S_FRONT_DEPLOY) \
	$(K8S_FRONT_SVC)

# add -f flag to each file
K8S_APP_FILES_FLAGS := $(foreach f,$(K8S_APP_MANIFESTS),-f $(f))

k8s-apply: k8s-apply-namespace
	kubectl apply $(K8S_APP_FILES_FLAGS)

k8s-delete:
	-kubectl delete $(K8S_APP_FILES_FLAGS) --ignore-not-found=true

k8s-apply-ingress:
	kubectl apply -f $(K8S_INGRESS)

k8s-delete-ingress:
	-kubectl delete -f $(K8S_INGRESS)

# Status / Logs / Wait
k8s-status:
	kubectl get deploy,pods,svc,hpa -l project=realestate
	kubectl get ingress

k8s-urls:
	@echo "Namespace: $(K8S_NS)"
	@echo ""
	@echo "Ingress:"
	@echo "  Frontend: http://localhost/"
	@echo "  API:      http://localhost/api (for health check add /health)"
	@echo "  CMS:      http://cms.localhost/"
	@echo ""
	@echo "Direct ports:"
	@echo "  Frontend: http://localhost:3000"
	@echo "  API:      http://localhost:5001"
	@echo "  CMS:      http://localhost:3333"

k8s-logs-frontend:
	kubectl -n $(K8S_NS) logs -l app=realestate-api --tail=$(K8S_LOG_TAIL) -f

k8s-logs-api:
	kubectl -n $(K8S_NS) logs -l app=realestate-api --tail=$(K8S_LOG_TAIL) -f

k8s-logs-mongo:
	kubectl -n $(K8S_NS) logs -l app=realestate-mongo --tail=$(K8S_LOG_TAIL) -f

k8s-wait-frontend:
	kubectl -n $(K8S_NS) rollout status deploy/realestate-frontend --timeout=$(K8S_TIMEOUT)s

k8s-wait-mongo:
	kubectl -n $(K8S_NS) rollout status deploy/realestate-mongo --timeout=$(K8S_TIMEOUT)s

k8s-wait-cms:
	kubectl -n $(K8S_NS) rollout status deploy/realestate-cms --timeout=$(K8S_TIMEOUT)s

k8s-wait-api:
	kubectl -n $(K8S_NS) rollout status deploy/realestate-api --timeout=$(K8S_TIMEOUT)s

k8s-wait-all: k8s-wait-frontend k8s-wait-mongo k8s-wait-cms k8s-wait-api

# One-shot reset
k8s-reset:
	$(MAKE) k8s-delete-ingress
	$(MAKE) k8s-delete
	$(MAKE) k8s-apply
	$(MAKE) k8s-disable-traefik
	$(MAKE) k8s-apply-ingress
	$(MAKE) k8s-wait-all
	$(MAKE) k8s-urls

k8s-help:
	@$(PRINT) ""
	@$(PRINT) "$(LIGHT_BLUE)==================================================$(RESET)"
	@$(PRINT) "$(GREEN)      Kubernetes (k3d) — Available Commands$(RESET)"
	@$(PRINT) "$(LIGHT_BLUE)==================================================$(RESET)"
	@$(PRINT) ""
	@$(PRINT) "$(YELLOW)Defaults:$(RESET)"
	@$(PRINT) "  $(GREEN)K3D_CLUSTER$(RESET)   = $(K3D_CLUSTER)"
	@$(PRINT) "  $(GREEN)K8S_NS$(RESET)        = $(K8S_NS)"
	@$(PRINT) "  $(GREEN)K8S_NAMESPACE$(RESET) = $(K8S_NAMESPACE)"
	@$(PRINT) "  $(GREEN)K8S_LOG_TAIL$(RESET)  = $(K8S_LOG_TAIL)   (override: make k8s-logs-api K8S_LOG_TAIL=50)"
	@$(PRINT) "  $(GREEN)K8S_TIMEOUT$(RESET)   = $(K8S_TIMEOUT)    (seconds, for rollout waits; override: make k8s-wait-all K8S_TIMEOUT=300)"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Cluster lifecycle:$(RESET)"
	@$(PRINT) "  $(GREEN)k8s-up$(RESET)                 - Create k3d cluster and configure kubectl context"
	@$(PRINT) "  $(GREEN)k8s-down$(RESET)               - Delete k3d cluster"
	@$(PRINT) "  $(GREEN)k8s-disable-traefik$(RESET)    - Disable k3s default Traefik (for ingress-nginx on :80)"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Deploy / reset:$(RESET)"
	@$(PRINT) "  $(GREEN)k8s-apply-namespace$(RESET)    - Create/apply $(K8S_NS) namespace"
	@$(PRINT) "  $(GREEN)k8s-apply$(RESET)              - Apply app manifests (mongo, api, cms, frontend) into $(K8S_NS)"
	@$(PRINT) "  $(GREEN)k8s-delete$(RESET)             - Delete app manifests from the cluster (ignore not found)"
	@$(PRINT) "  $(GREEN)k8s-reset$(RESET)              - Full reset: delete ingress + app, apply app, disable traefik, apply ingress, wait"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Ingress (optional local routing):$(RESET)"
	@$(PRINT) "  $(GREEN)k8s-apply-ingress$(RESET)      - Apply ingress rules from $(K8S_INGRESS)"
	@$(PRINT) "  $(GREEN)k8s-delete-ingress$(RESET)     - Delete ingress rules (ignore not found)"
	@$(PRINT) "  $(GREEN)k8s-urls$(RESET)               - Print local URLs (ingress + direct ports)"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Observability:$(RESET)"
	@$(PRINT) "  $(GREEN)k8s-status$(RESET)             - Show deploy/pods/svc/hpa/ingress for project=realestate"
	@$(PRINT) "  $(GREEN)k8s-wait-all$(RESET)           - Wait for all deployments to become Ready (uses K8S_TIMEOUT)"
	@$(PRINT) "  $(GREEN)k8s-wait-api$(RESET)           - Wait for API deployment rollout (uses K8S_TIMEOUT)"
	@$(PRINT) "  $(GREEN)k8s-wait-frontend$(RESET)      - Wait for frontend deployment rollout (uses K8S_TIMEOUT)"
	@$(PRINT) "  $(GREEN)k8s-wait-cms$(RESET)           - Wait for CMS deployment rollout (uses K8S_TIMEOUT)"
	@$(PRINT) "  $(GREEN)k8s-wait-mongo$(RESET)         - Wait for Mongo deployment rollout (uses K8S_TIMEOUT)"
	@$(PRINT) ""
	
	@$(PRINT) "$(YELLOW)Logs:$(RESET)"
	@$(PRINT) "  $(GREEN)k8s-logs-api$(RESET)           - Tail API logs (namespace: $(K8S_NS), uses K8S_LOG_TAIL)"
	@$(PRINT) "  $(GREEN)k8s-logs-frontend$(RESET)      - Tail frontend logs (namespace: $(K8S_NS), uses K8S_LOG_TAIL)"
	@$(PRINT) "  $(GREEN)k8s-logs-mongo$(RESET)         - Tail Mongo logs (namespace: $(K8S_NS), uses K8S_LOG_TAIL)"
	@$(PRINT) ""

	@$(PRINT) "$(PURPLE)Quick start (recommended):$(RESET)"
	@$(PRINT) "  $(GREEN)make k8s-up$(RESET)"
	@$(PRINT) "  $(GREEN)make k8s-reset$(RESET)"
	@$(PRINT) "  $(GREEN)make k8s-status$(RESET)"
	@$(PRINT) "  $(GREEN)make k8s-urls$(RESET)"
	@$(PRINT) ""
	@$(PRINT) "$(PURPLE)Usage:$(RESET)"
	@$(PRINT) "  make <command>"
	@$(PRINT) ""
