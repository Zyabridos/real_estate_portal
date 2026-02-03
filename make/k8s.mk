K3D_CLUSTER 	 ?= realestate

K8S_MONGO_DEPLOY ?= k8s/mongo/deployment.yaml
K8S_MONGO_SVC    ?= k8s/mongo/service.yaml

K8S_API_DEPLOY   ?= k8s/api/deployment.yaml
K8S_API_SVC      ?= k8s/api/service.yaml

K8S_CMS_DEPLOY   ?= k8s/cms/deployment.yaml
K8S_CMS_SVC      ?= k8s/cms/service.yaml

K8S_FRONT_DEPLOY ?= k8s/frontend/deployment.yaml
K8S_FRONT_SVC    ?= k8s/frontend/service.yaml

K8S_INGRESS      ?= k8s/ingress/ingress.yaml

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

# Apply / Delete
k8s-apply:
	kubectl apply -f $(K8S_MONGO_DEPLOY)
	kubectl apply -f $(K8S_MONGO_SVC)

	kubectl apply -f $(K8S_API_DEPLOY)
	kubectl apply -f $(K8S_API_SVC)

	kubectl apply -f $(K8S_CMS_DEPLOY)
	kubectl apply -f $(K8S_CMS_SVC)

	kubectl apply -f $(K8S_FRONT_DEPLOY)
	kubectl apply -f $(K8S_FRONT_SVC)

k8s-delete:
	-kubectl delete -f $(K8S_FRONT_DEPLOY)
	-kubectl delete -f $(K8S_FRONT_SVC)

	-kubectl delete -f $(K8S_CMS_DEPLOY)
	-kubectl delete -f $(K8S_CMS_SVC)

	-kubectl delete -f $(K8S_API_DEPLOY)
	-kubectl delete -f $(K8S_API_SVC)

	-kubectl delete -f $(K8S_MONGO_DEPLOY)
	-kubectl delete -f $(K8S_MONGO_SVC)

k8s-apply-ingress:
	kubectl apply -f $(K8S_INGRESS)

k8s-delete-ingress:
	-kubectl delete -f $(K8S_INGRESS)

# Status / Logs / Wait
k8s-status:
	kubectl get deploy,pods,svc,hpa -l project=realestate
	kubectl get ingress

k8s-urls:
	@echo "Ingress:"
	@echo "  Frontend: http://localhost/"
	@echo "  API:      http://localhost/api (for health check add /health)"
	@echo "  CMS:      http://cms.localhost/"
	@echo ""
	@echo "Direct ports:"
	@echo "  Frontend: http://localhost:3000"
	@echo "  API:      http://localhost:5001"
	@echo "  CMS:      http://localhost:3333"

k8s-logs-api:
	kubectl logs -l app=realestate-api --tail=200 -f

k8s-logs-mongo:
	kubectl logs -l app=realestate-mongo --tail=200 -f

k8s-wait-frontend:
	kubectl rollout status deploy/realestate-frontend --timeout=180s

k8s-wait-mongo:
	kubectl rollout status deploy/realestate-mongo --timeout=180s

k8s-wait-cms:
	kubectl rollout status deploy/realestate-cms --timeout=180s

k8s-wait-api:
	kubectl rollout status deploy/realestate-api --timeout=180s

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
