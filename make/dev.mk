# Local development commands. No variables required

dev-frontend:
	cd $(FRONTEND_DIR) && npm run dev

dev-backend:
	cd $(BACKEND_DIR)/src/RealEstate.Api && dotnet watch run

dev-cms:
	cd $(CMS_DIR) && npm run dev

dev:
	make dev-backend && make dev-frontend && make dev-cms

reset-backend: ## Hard reset backend (NuGet cache + bin/obj) and build
	cd $(BACKEND_DIR) && \
    	dotnet nuget locals all --clear && \
    	rm -rf src/**/bin src/**/obj && \
    	dotnet restore --disable-parallel && \
    	dotnet build

reset-frontend: ## Hard reset frontend (node_modules reinstall)
	cd $(FRONTEND_DIR) && \
	rm -rf node_modules package-lock.json && \
	npm install

test-backend:
	@echo "$(LIGHT_BLUE)Starting tests for backend...$(RESET)"
	cd $(BACKEND_DIR) && dotnet test RealEstate.slnx

test-backend-coverage:
	@echo "$(LIGHT_BLUE)Running backend tests with coverage...$(RESET)"
	cd $(BACKEND_DIR) && \
    	rm -rf ./TestResults ./coverage-report && \
    	dotnet test ./RealEstate.slnx -c Release \
    		--collect:"XPlat Code Coverage;Format=cobertura" \
    		--results-directory ./TestResults && \
    	reportgenerator \
    		-reports:"TestResults/**/coverage.cobertura.xml" \
    		-targetdir:"coverage-report" \
    		-reporttypes:"Html;HtmlSummary" && \
    	( command -v open >/dev/null 2>&1 && open coverage-report/index.html || true ) && \
    	( command -v xdg-open >/dev/null 2>&1 && xdg-open coverage-report/index.html || true )
		
test-frontend-unit:
	@echo "$(LIGHT_BLUE)Running frontend unit tests...$(RESET)"
	cd $(FRONTEND_DIR) && npm run test:unit
	
test-frontend-e2e:
	@echo "$(LIGHT_BLUE)Running frontend e2e tests...$(RESET)"
	cd $(FRONTEND_DIR)/tests/e2e && npm run test

test: test-backend test-frontend-unit test-frontend-e2e