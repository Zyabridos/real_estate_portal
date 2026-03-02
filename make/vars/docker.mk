COMPOSE := docker compose --env-file $(DEVELOPMENT_DOCKER_ENV_FILE)

# names in docker-compose.yml
SERVICE_BACKEND  ?= backend
SERVICE_FRONTEND ?= frontend
SERVICE_MONGO    ?= mongodb
SERVICE_CMS      ?= cms

CORE_SERVICES ?= $(SERVICE_BACKEND) $(SERVICE_FRONTEND) $(SERVICE_MONGO)