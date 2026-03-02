FRONTEND_DIR ?= frontend
BACKEND_DIR  ?= backend
CMS_DIR      ?= cms

INFRA_DIR     		   = infrastructure
TERRAFORM_DIR 		   = $(INFRA_DIR)/terraform
ANSIBLE_DIR   		   = $(INFRA_DIR)/ansible
ANSIBLE_INVENTORY_DIR  = $(ANSIBLE_DIR)/inventories/generated
ANSIBLE_INVENTORY_PATH = $(ANSIBLE_INVENTORY_DIR)/inventory.$(ENV)-$(STACK).ini

K8S_DIR ?= k8s