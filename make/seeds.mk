# Requires variables: BACKEND_PORT, BACKEND_URL

seed-agencies:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/seed/seed_agencies.sh

seed-brokers:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/seed/seed_brokers.sh

seed-properties:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/seed/seed_properties.sh

seed-leads:
	@BACKEND_PORT=$(BACKEND_PORT) BACKEND_URL=$(BACKEND_URL) ./scripts/bash/seed/seed_leads.sh

seed: seed-agencies seed-brokers seed-properties seed-leads
