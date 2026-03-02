# Infra env selection
ENV   ?= prod
STACK ?= blue

TF_WORKSPACE ?= $(ENV)-$(STACK)

FRONTEND_BUILD_ENV_FILE ?= .env

DEVELOPMENT_DOCKER_ENV_FILE ?= .env.development
