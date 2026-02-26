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
API_LIVENESS_PATH  ?= /api/health/liveness
API_READINESS_PATH ?= /api/health/readiness

DEV_FRONT_HOST    ?= localhost
DEV_CMS_HOST      ?= cms.localhost

PROD_FRONT_HOST   ?= www.realestateproject.casa
PROD_CMS_HOST     ?= cms.realestateproject.casa

# -----------------------------
# k3d cluster ports / mappings
K3D_SERVERS            ?= 1
K3D_AGENTS             ?= 1
K3D_API_HOST_PORT      ?= 6550

K3D_LB_HTTP_HOST_PORT  ?= 80
K3D_LB_HTTPS_HOST_PORT ?= 443
K3D_LB_HTTP_CLUSTER_PORT  ?= 80
K3D_LB_HTTPS_CLUSTER_PORT ?= 443

# k3s args
K3D_K3S_ARGS ?= --k3s-arg "--disable=traefik@server:*"

# -----------------------------
# Ingress local port (for smoke via localhost)
K8S_INGRESS_LOCAL_HTTP_PORT ?= $(K3D_LB_HTTP_HOST_PORT)

# -----------------------------
# Service ports (inside cluster) - do not forget to change if these will be changed in Service manifests (I might...)
K8S_SVC_API_PORT     ?= 5000
K8S_SVC_FRONT_PORT   ?= 80
K8S_SVC_CMS_PORT     ?= 80
K8S_SVC_MONGO_PORT   ?= 27017

# -----------------------------
# Port-forward local ports
K8S_PF_API_LOCAL_PORT    ?= 5000
K8S_PF_FRONT_LOCAL_PORT  ?= 8080
K8S_PF_CMS_LOCAL_PORT    ?= 3333
K8S_PF_MONGO_LOCAL_PORT  ?= 27017

k8s-set-ns:
	kubectl config set-context --current --namespace=$(K8S_NS)

# -----------------------------
# Cluster lifecycle
k8s-up:
	@echo "$(BLUE)Creating k3d cluster: $(K3D_CLUSTER)$(RESET)\n"
	k3d cluster create $(K3D_CLUSTER) --servers $(K3D_SERVERS) --agents $(K3D_AGENTS) --api-port $(K3D_API_HOST_PORT) \
		-p "$(K3D_LB_HTTP_HOST_PORT):$(K3D_LB_HTTP_CLUSTER_PORT)@loadbalancer" \
		-p "$(K3D_LB_HTTPS_HOST_PORT):$(K3D_LB_HTTPS_CLUSTER_PORT)@loadbalancer" \
		$(K3D_K3S_ARGS)
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
# Apply / delete / reset... yeah, I am that lazy sometimes..
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
# Wait
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
	@echo "$(YELLOW)API -> http://localhost:$(K8S_PF_API_LOCAL_PORT) $(RESET)\n"
	kubectl port-forward svc/realestate-api-svc $(K8S_PF_API_LOCAL_PORT):$(K8S_SVC_API_PORT)

k8s-pf-frontend:
	@echo "$(YELLOW)Frontend -> http://localhost:$(K8S_PF_FRONT_LOCAL_PORT) $(RESET)\n"
	kubectl port-forward svc/realestate-frontend-svc $(K8S_PF_FRONT_LOCAL_PORT):$(K8S_SVC_FRONT_PORT)

k8s-pf-cms:
	@echo "$(YELLOW)CMS -> http://localhost:$(K8S_PF_CMS_LOCAL_PORT) $(RESET)\n"
	kubectl port-forward svc/realestate-cms-svc $(K8S_PF_CMS_LOCAL_PORT):$(K8S_SVC_CMS_PORT)

k8s-pf-mongo:
	@echo "$(YELLOW)Mongo -> mongodb://localhost:$(K8S_PF_MONGO_LOCAL_PORT) $(RESET)\n"
	kubectl port-forward svc/realestate-mongo $(K8S_PF_MONGO_LOCAL_PORT):$(K8S_SVC_MONGO_PORT)

show-k8s-config:
	@echo -e "$(BOLD)K3D_CLUSTER$(RESET)=$(K3D_CLUSTER)"
	@echo -e "$(BOLD)K8S_NS$(RESET)=$(K8S_NS)"
	@echo -e "$(BOLD)K8S_DIR$(RESET)=$(K8S_DIR)"
	@echo -e "$(BOLD)K8S_BASE$(RESET)=$(K8S_BASE)"
	@echo -e "$(BOLD)K8S_OVERLAYS$(RESET)=$(K8S_OVERLAYS)"
	@echo -e "$(BOLD)K8S_ENV$(RESET)=$(K8S_ENV)"
	@echo -e "$(BOLD)K8S_KUSTOMIZE_DIR$(RESET)=$(K8S_KUSTOMIZE_DIR)"
	@echo -e "$(BOLD)K8S_LOG_TAIL$(RESET)=$(K8S_LOG_TAIL)"
	@echo -e "$(BOLD)K8S_TIMEOUT$(RESET)=$(K8S_TIMEOUT)"
	@echo -e "$(BOLD)API_LIVENESS_PATH$(RESET)=$(API_LIVENESS_PATH)"
	@echo -e "$(BOLD)API_READINESS_PATH$(RESET)=$(API_READINESS_PATH)"
	@echo -e "$(BOLD)DEV_FRONT_HOST$(RESET)=$(DEV_FRONT_HOST)"
	@echo -e "$(BOLD)DEV_CMS_HOST$(RESET)=$(DEV_CMS_HOST)"
	@echo -e "$(BOLD)PROD_FRONT_HOST$(RESET)=$(PROD_FRONT_HOST)"
	@echo -e "$(BOLD)PROD_CMS_HOST$(RESET)=$(PROD_CMS_HOST)"
	@echo -e "$(BOLD)K3D_SERVERS$(RESET)=$(K3D_SERVERS)"
	@echo -e "$(BOLD)K3D_AGENTS$(RESET)=$(K3D_AGENTS)"
	@echo -e "$(BOLD)K3D_API_HOST_PORT$(RESET)=$(K3D_API_HOST_PORT)"
	@echo -e "$(BOLD)K3D_LB_HTTP_HOST_PORT$(RESET)=$(K3D_LB_HTTP_HOST_PORT)"
	@echo -e "$(BOLD)K3D_LB_HTTPS_HOST_PORT$(RESET)=$(K3D_LB_HTTPS_HOST_PORT)"
	@echo -e "$(BOLD)K3D_LB_HTTP_CLUSTER_PORT$(RESET)=$(K3D_LB_HTTP_CLUSTER_PORT)"
	@echo -e "$(BOLD)K3D_LB_HTTPS_CLUSTER_PORT$(RESET)=$(K3D_LB_HTTPS_CLUSTER_PORT)"
	@echo -e "$(BOLD)K3D_K3S_ARGS$(RESET)=$(K3D_K3S_ARGS)"
	@echo -e "$(BOLD)K8S_INGRESS_LOCAL_HTTP_PORT$(RESET)=$(K8S_INGRESS_LOCAL_HTTP_PORT)"
	@echo -e "$(BOLD)K8S_SVC_API_PORT$(RESET)=$(K8S_SVC_API_PORT)"
	@echo -e "$(BOLD)K8S_SVC_FRONT_PORT$(RESET)=$(K8S_SVC_FRONT_PORT)"
	@echo -e "$(BOLD)K8S_SVC_CMS_PORT$(RESET)=$(K8S_SVC_CMS_PORT)"
	@echo -e "$(BOLD)K8S_SVC_MONGO_PORT$(RESET)=$(K8S_SVC_MONGO_PORT)"
	@echo -e "$(BOLD)K8S_PF_API_LOCAL_PORT$(RESET)=$(K8S_PF_API_LOCAL_PORT)"
	@echo -e "$(BOLD)K8S_PF_FRONT_LOCAL_PORT$(RESET)=$(K8S_PF_FRONT_LOCAL_PORT)"
	@echo -e "$(BOLD)K8S_PF_CMS_LOCAL_PORT$(RESET)=$(K8S_PF_CMS_LOCAL_PORT)"
	@echo -e "$(BOLD)K8S_PF_MONGO_LOCAL_PORT$(RESET)=$(K8S_PF_MONGO_LOCAL_PORT)"
