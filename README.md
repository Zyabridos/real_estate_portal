

# Real Estate Mini Portal

Educational full-stack project for learning **Vue 3**, **.NET Web API (C#)**, and **Sanity CMS** in a real estate domain.

The project is inspired by data-driven listing systems used for managing property inventories, brokers, and customer inquiries.

---

## Badges
[![Backend Integration Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-integration-tests.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-integration-tests.yml) \
[![Backend Unit Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-unit-tests.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-unit-tests.yml) \
[![Frontend Unit Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-unit.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-unit.yml) \
[![Frontend E2E (Playwright) Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-e2e.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-e2e.yml)



## Tech Stack

### Frontend
- Vue 3
- Vue Router
- Pinia
- Tailwind CSS
- TypeScript
- Vitest (unit tests)
- Playwright (E2E tests)

### Backend
- .NET 10 (ASP.NET Core Web API)
- MongoDB
- xUnit
- Testcontainers (integration tests)

### CMS
- Sanity v3
- GROQ
- Portable Text

---


## Prerequisites

For local development:
- Docker + Docker Compose

For provisioning/deployment (infrastructure):
- Terraform
- Ansible
- SSH access to provisioned hosts (SSH key)

Optional but recommended:
- `make`

## Project Structure

```
  frontend/          # Vue application (UI, routing, state management)
  backend/           # .NET Web API (business logic, database)
  cms/               # Sanity Studio (editorial content)
  infrastructure /   # Terraform + Ansible (provisioning + deployment)
  
  make/              # Makefile modules (e.g., docker targets)
  scripts/           # Utilities and automation (seed scripts, helpers)
  docker-compose.yml # Local dev stack (MongoDB + API + UI + optional CMS)
  Makefile           # Entry point for common dev commands
 
  docker-compose.yml
  Makefile
  LICENSE
```
## Environment Variables

This repo uses separate env templates depending on the workflow.

Rename one of the templates and adjust values:

```bash
mv .env.dev.example .env.dev     # default Docker dev environment
```
```bash
mv .env.e2e.example .env.e2e     # local E2E environment (isolated DB)
```
```bash
mv .env.local.example .env.local # optional: run apps on host (non-Docker). Recommended: run everything via Docker for consistency.
```
## Which one should I use?

`.env.dev` — main local development with Docker Compose

`.env.e2e` — isolated environment for E2E runs (safe to reseed/reset)

`.env.local` — optional overrides when running frontend/backend on the host machine

## Development Commands

Common development tasks are automated via `Makefile`.

To see the full list of available commands:
```bash
make help
```

Typical workflow:
```bash
make up-d    # Start all services in detached mode
make down    # Stop services 
make restart # Restart services 
make logs    # View service logs
```

## Testing
Run all backend tests:
```bash
make test-back
```
Or directly:
```bash
dotnet test backend/RealEstate.slnx
```

## Notes on Configuration
- Frontend API calls are proxied via Vite (/api → backend)
- Backend communicates with MongoDB via Docker network
- MongoDB runs only once per test suite (Testcontainers)
- CMS is optional and started via Docker profiles
- Swagger UI is available in development mode