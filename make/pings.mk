# Requires variables: BACKEND_PORT, BACKEND_URL

ping-api:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/pings/ping_api.sh

ping-entities:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/pings/ping_entities.sh "$(ENTITY)"

ping-entity:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/pings/ping_entity_by_id.sh "$(ENTITY)" "$(ID)"

# aliases
ping-properties:
	@$(MAKE) ping-entities ENTITY=properties

ping-property:
	@$(MAKE) ping-entity ENTITY=properties ID="$(ID)"

ping-brokers:
	@$(MAKE) ping-entities ENTITY=brokers

ping-broker:
	@$(MAKE) ping-entity ENTITY=brokers ID="$(ID)"

ping-leads:
	@$(MAKE) ping-entities ENTITY=leads

ping-lead:
	@$(MAKE) ping-entity ENTITY=leads ID="$(ID)"

smoke-api: ping-api ping-properties ping-brokers ping-leads
