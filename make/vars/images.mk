GIT_SHA   := $(shell git rev-parse --short HEAD)
IMAGE_TAG ?= sha-$(GIT_SHA)

FRONTEND_REPO ?= zyabridos/real_estate_prod_frontend
BACKEND_REPO  ?= zyabridos/real_estate_prod_backend
CMS_REPO      ?= zyabridos/real_estate_prod_cms

FRONTEND_IMAGE := $(FRONTEND_REPO):$(IMAGE_TAG)
BACKEND_IMAGE  := $(BACKEND_REPO):$(IMAGE_TAG)
CMS_IMAGE      := $(CMS_REPO):$(IMAGE_TAG)
