RESET       = \033[0m
BOLD        = \033[1m
RED         = \033[1;31m
GREEN       = \033[1;32m
YELLOW      = \033[1;33m
BLUE        = \033[1;34m
PURPLE      = \033[1;35m
LIGHT_BLUE  = \033[1;36m

PRINT = printf '%b\n'

# Directories
INFRA_DIR     ?= infrastructure
TERRAFORM_DIR ?= $(INFRA_DIR)/terraform
ANSIBLE_DIR   ?= $(INFRA_DIR)/ansible

# Env selection
ENV   ?= prod
STACK ?= blue

TF_WORKSPACE ?= $(ENV)-$(STACK)
TFVARS       ?= terraform.$(ENV).$(STACK).tfvars
TFVARS_PATH  := $(abspath $(TERRAFORM_DIR)/$(TFVARS))

# Image tagging (same tag for backend/frontend/cms)
GIT_SHA   := $(shell git rev-parse --short HEAD)
IMAGE_TAG ?= sha-$(GIT_SHA)

FRONTEND_REPO ?= zyabridos/real_estate_prod_frontend
BACKEND_REPO  ?= zyabridos/real_estate_prod_backend
CMS_REPO      ?= zyabridos/real_estate_prod_cms

FRONTEND_IMAGE := $(FRONTEND_REPO):$(IMAGE_TAG)
BACKEND_IMAGE  := $(BACKEND_REPO):$(IMAGE_TAG)
CMS_IMAGE      := $(CMS_REPO):$(IMAGE_TAG)

# Docker compose env (local dev)
DOCKER_ENV_FILE ?= .env.development

# Frontend build-time env for Vite build args (production build)
FRONTEND_ENV_FILE ?= .env

include make/vars/ui.mk

include make/vars/paths.mk
include make/vars/env.mk
include make/vars/images.mk
include make/vars/docker.mk
include make/vars/infra.mk
include make/vars/k8s.mk