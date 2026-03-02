K3D_CLUSTER       ?= realestate
K8S_NS            ?= realestate
K8S_DIR           ?= k8s
K8S_BASE          ?= $(K8S_DIR)/base
K8S_OVERLAYS      ?= $(K8S_DIR)/overlays

K8S_ENV           ?= dev
K8S_KUSTOMIZE_DIR ?= $(K8S_OVERLAYS)/$(K8S_ENV)

K8S_LOG_TAIL      ?= 200
K8S_TIMEOUT       ?= 300

API_LIVENESS_PATH  ?= /api/health/liveness
API_READINESS_PATH ?= /api/health/readiness

DEV_FRONT_HOST    ?= localhost
DEV_CMS_HOST      ?= cms.localhost

PROD_FRONT_HOST   ?= www.realestateproject.casa
PROD_CMS_HOST     ?= cms.realestateproject.casa

K3D_SERVERS            ?= 1
K3D_AGENTS             ?= 1
K3D_API_HOST_PORT      ?= 6550

K3D_LB_HTTP_HOST_PORT  ?= 80
K3D_LB_HTTPS_HOST_PORT ?= 443
K3D_LB_HTTP_CLUSTER_PORT  ?= 80
K3D_LB_HTTPS_CLUSTER_PORT ?= 443

K3D_K3S_ARGS ?= --k3s-arg "--disable=traefik@server:*"

K8S_INGRESS_LOCAL_HTTP_PORT ?= $(K3D_LB_HTTP_HOST_PORT)

K8S_SVC_API_PORT     ?= 5000
K8S_SVC_FRONT_PORT   ?= 80
K8S_SVC_CMS_PORT     ?= 80
K8S_SVC_MONGO_PORT   ?= 27017

K8S_PF_API_LOCAL_PORT    ?= 5000
K8S_PF_FRONT_LOCAL_PORT  ?= 8080
K8S_PF_CMS_LOCAL_PORT    ?= 3333
K8S_PF_MONGO_LOCAL_PORT  ?= 27017