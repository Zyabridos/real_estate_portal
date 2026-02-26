# Local development commands. No variables required

dev-frontend:
	cd frontend && npm run dev

dev-backend:
	cd backend/src/RealEstate.Api && dotnet watch run

dev-cms:
	cd cms && npm run dev

dev:
	make dev-backend && make dev-frontend && make dev-cms

backend-full-rebuild:
	cd backend && \
    dotnet nuget locals all --clear && \
    rm -rf src/**/bin src/**/obj && \
    dotnet restore --disable-parallel && \
    dotnet build && \
    dotnet run --project src/RealEstate.Api/RealEstate.Api.csproj

frontend-clean-install:
	cd frontend && \
	rm -rf node_modules package-lock.json && \
	npm install

test-backend:
	@echo "$(LIGHT_BLUE)Starting tests for backend...$(RESET)"
	cd backend && dotnet test RealEstate.slnx

test-backend-coverage:
	@echo "$(LIGHT_BLUE)Running backend tests with coverage...$(RESET)"
	cd backend && \
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