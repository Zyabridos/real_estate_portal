K3D_CLUSTER ?= realestate

K8S_API_YAML ?= k8s/api/deployment.yaml
K8S_CMS_YAML ?= k8s/cms/deployment.yaml
K8S_FRONT_YAML ?= k8s/frontend/deployment.yaml

LOCAL_API_PORT ?= 5001
LOCAL_CMS_PORT ?= 3333
LOCAL_FRONTEND_PORT ?= 3000
CONTAINER_HTTP_PORT ?= 80

k8s-down:
	k3d cluster delete $(K3D_CLUSTER)

k8s-up:
	k3d cluster create $(K3D_CLUSTER) --servers 1 --agents 1 --api-port 6550 -p "80:80@loadbalancer" -p "443:443@loadbalancer"

k8s-apply:
	kubectl apply -f $(K8S_API_YAML)
	kubectl apply -f $(K8S_CMS_YAML)
	kubectl apply -f $(K8S_FRONT_YAML)

k8s-delete:
	kubectl delete -f $(K8S_API_YAML) --ignore-not-found
	kubectl delete -f $(K8S_CMS_YAML) --ignore-not-found
	kubectl delete -f $(K8S_FRONT_YAML) --ignore-not-found

k8s-status:
	kubectl get deploy,pods,hpa -l project=realestate

k8s-front-describe:
	kubectl describe pods -l app=realestate-frontend

k8s-logs-api:
	kubectl logs -l app=realestate-api --tail=200 -f

k8s-logs-cms:
	kubectl logs -l app=realestate-cms --tail=200 -f

k8s-logs-front:
	kubectl logs -l app=realestate-frontend --tail=200 -f

# Port-forwarding (for now) - just to check that deployments works
k8s-pf-api:
	kubectl port-forward deployment/realestate-api $(LOCAL_API_PORT):$(CONTAINER_HTTP_PORT)

k8s-pf-cms:
	kubectl port-forward deployment/realestate-cms $(LOCAL_CMS_PORT):$(CONTAINER_HTTP_PORT)

k8s-pf-front:
	kubectl port-forward deployment/realestate-frontend $(LOCAL_FRONTEND_PORT):$(CONTAINER_HTTP_PORT)
