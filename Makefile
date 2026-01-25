include make/dev.mk
include make/pings.mk
include make/seeds.mk
include make/docker.mk
include make/infrastructure.mk

SHELL := /bin/bash
.ONESHELL:
.SHELLFLAGS := -eu -o pipefail -c
MAKEFLAGS += --no-builtin-rules
.DEFAULT_GOAL := help

# Project services
BACKEND        = backend
FRONTEND       = frontend
CORE_SERVICES  = mongodb backend frontend
CMS_SERVICE    = cms

# Ports and URLs (single source of truth is scripts/lib/config.sh,
# but we pass defaults from Makefile too if you want)
BACKEND_PORT   ?= 5055
FRONTEND_PORT  ?= 3000
MONGO_PORT     ?= 27017
BACKEND_URL    ?= http://localhost:$(BACKEND_PORT)

# ANSI colors & styles
RESET       = \033[0m
BOLD        = \033[1m

RED         = \033[1;31m
GREEN       = \033[1;32m
YELLOW      = \033[1;33m
BLUE        = \033[1;34m
PURPLE      = \033[1;35m
LIGHT_BLUE  = \033[1;36m

PRINT = printf '%b\n'

.PHONY: help
help:
	@$(PRINT) ""
	@$(PRINT) "$(LIGHT_BLUE)==================================================$(RESET)"
	@$(PRINT) "$(GREEN)      Real Estate Project — Available Commands$(RESET)"
	@$(PRINT) "$(LIGHT_BLUE)==================================================$(RESET)"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Local development:$(RESET)"
	@$(PRINT) "  $(GREEN)dev-backend$(RESET)            - Run backend locally (dotnet watch)"
	@$(PRINT) "  $(GREEN)dev-frontend$(RESET)           - Run frontend locally (Vite)"
	@$(PRINT) "  $(GREEN)dev-cms$(RESET)                - Run Sanity Studio locally"
	@$(PRINT) "  $(GREEN)dev$(RESET)                    - Run backend + frontend + cms"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)API / Ports:$(RESET)"
	@$(PRINT) "  $(GREEN)ping-api$(RESET)                              - Check backend health endpoint"
	@$(PRINT) "  $(GREEN)ping-entities ENTITY=<name>$(RESET)           - Fetch list (properties | brokers | leads)"
	@$(PRINT) "  $(GREEN)ping-entity ENTITY=<name> ID=<guid>$(RESET)   - Fetch by id"
	@$(PRINT) "  $(GREEN)smoke-api$(RESET)                             - Run basic API smoke checks"
	@$(PRINT) "  $(GREEN)Aliases:$(RESET) ping-properties/ping-property, ping-brokers/ping-broker, ping-leads/ping-lead"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Reset / bootstrap (local):$(RESET)"
	@$(PRINT) "  $(GREEN)backend-full-rebuild$(RESET)   - Full backend reset (NuGet, bin/obj, rebuild & run)"
	@$(PRINT) "  $(GREEN)frontend-clean-install$(RESET) - Full frontend reset (node_modules reinstall)"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Testing:$(RESET)"
	@$(PRINT) "  $(GREEN)test-back$(RESET)              - Run backend test suite"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Docker commands:$(RESET)"
	@$(PRINT) "  $(GREEN)build$(RESET)                  - Build Docker images"
	@$(PRINT) "  $(GREEN)up$(RESET)                     - Start all services (foreground)"
	@$(PRINT) "  $(GREEN)up-d$(RESET)                   - Start all services (background)"
	@$(PRINT) "  $(GREEN)down$(RESET)                   - Stop and remove all containers"
	@$(PRINT) "  $(GREEN)rebuild$(RESET)                - Stop, rebuild and restart all services"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Docker service management:$(RESET)"
	@$(PRINT) "  $(GREEN)restart$(RESET)                - Restart core services ($(CORE_SERVICES))"
	@$(PRINT) "  $(GREEN)restart-backend$(RESET)        - Restart backend container"
	@$(PRINT) "  $(GREEN)restart-frontend$(RESET)       - Restart frontend container"
	@$(PRINT) "  $(GREEN)restart-db$(RESET)             - Restart mongodb container"
	@$(PRINT) "  $(GREEN)restart-with-cms$(RESET)       - Restart core services + cms"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Shell inside containers:$(RESET)"
	@$(PRINT) "  $(GREEN)sh-backend$(RESET)             - Shell into backend container"
	@$(PRINT) "  $(GREEN)sh-frontend$(RESET)            - Shell into frontend container"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Cleanup:$(RESET)"
	@$(PRINT) "  $(GREEN)prune$(RESET)                  - Remove unused Docker resources"
	@$(PRINT) "  $(GREEN)clean$(RESET)                  - Full Docker cleanup ($(RED)danger!$(RESET))"
	@$(PRINT) ""

	@$(PRINT) "$(YELLOW)Infrastructure (Terraform + Ansible):$(RESET)"
	@$(MAKE) -s infra-help
	@$(PRINT) ""

	@$(PRINT) "$(PURPLE)Usage:$(RESET)"
	@$(PRINT) "  make <command>"
	@$(PRINT) ""

