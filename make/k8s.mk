K3D_CLUSTER 	 ?= realestate

K8S_MONGO_DEPLOY ?= k8s/mongo/deployment.yaml
K8S_MONGO_SVC    ?= k8s/mongo/service.yaml

K8S_API_DEPLOY   ?= k8s/api/deployment.yaml
K8S_API_SVC      ?= k8s/api/service.yaml

K8S_CMS_DEPLOY   ?= k8s/cms/deployment.yaml
K8S_CMS_SVC      ?= k8s/cms/service.yaml

K8S_FRONT_DEPLOY ?= k8s/frontend/deployment.yaml
K8S_FRONT_SVC    ?= k8s/frontend/service.yaml

k8s-down:
	k3d cluster delete $(K3D_CLUSTER)

k8s-up:
	k3d cluster create $(K3D_CLUSTER) --servers 1 --agents 1 --api-port 6550 \
		-p "80:80@loadbalancer" -p "443:443@loadbalancer" \
		-p "3000:3000@loadbalancer" -p "5001:5001@loadbalancer" -p "3333:3333@loadbalancer"
	$(MAKE) k8s-use-context

k8s-use-context:
	k3d kubeconfig merge $(K3D_CLUSTER) -s
	kubectl config current-context
	kubectl cluster-info

k8s-apply:
	kubectl apply -f $(K8S_MONGO_DEPLOY)
	kubectl apply -f $(K8S_MONGO_SVC)

	kubectl apply -f $(K8S_API_DEPLOY)
	kubectl apply -f $(K8S_API_SVC)

	kubectl apply -f $(K8S_CMS_DEPLOY)
	kubectl apply -f $(K8S_CMS_SVC)

	kubectl apply -f $(K8S_FRONT_DEPLOY)
	kubectl apply -f $(K8S_FRONT_SVC)

k8s-status:
	kubectl get deploy,pods,svc,hpa -l project=realestate

k8s-urls:
	@echo "Frontend: http://localhost:3000"
	@echo "API:      http://localhost:5001"
	@echo "CMS:      http://localhost:3333"

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
