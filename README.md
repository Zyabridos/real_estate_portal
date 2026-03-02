

# Real Estate Mini Portal

Educational full-stack project for learning **Vue 3**, **.NET Web API (C#)**, and **Sanity CMS** in a real estate domain. 

This is as well a playground for automation—because turning “do it manually” into “make it run itself” is oddly satisfying.

The project is inspired by data-driven listing systems used for managing property inventories, brokers, and customer inquiries.

---

## Badges
[![Maintainability](https://qlty.sh/gh/Zyabridos/projects/real_estate_portal/maintainability.svg)](https://qlty.sh/gh/Zyabridos/projects/real_estate_portal) \
[![Backend Integration Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-integration-tests.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-integration-tests.yml) \
[![Backend Unit Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-unit-tests.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-unit-tests.yml) \
[![Frontend Unit Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-unit.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-unit.yml) \
[![Frontend E2E (Playwright) Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-e2e.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/frontend-e2e.yml) \
[![Push images to Docker Hub](https://github.com/Zyabridos/real_estate_portal/actions/workflows/docker-push.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/docker-push.yml) \
[![Deployment to production](https://github.com/Zyabridos/real_estate_portal/actions/workflows/deploy-prod.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/deploy-prod.yml)



## Tech Stack

**Frontend**
- Vue 3, TypeScript, Vite
- Vue Router, Pinia
- Tailwind CSS
- Vitest (unit), Playwright (E2E)

**Backend**
- .NET 10 (ASP.NET Core Web API)
- MongoDB
- FluentValidation, AutoMapper
- xUnit + Testcontainers (integration tests)

**CMS**
- Sanity v3 (Portable Text + GROQ)

**Infrastructure / Delivery**
- Docker / Docker Compose (local dev)
- Kubernetes (k3s) + Kustomize (source of truth for prod manifests)
- Terraform (Hetzner Cloud)
- Ansible (k3s bootstrap, addons, deploy + verification)
- Blue/Green: `prod-blue` and `prod-green`

---

## Repository Structure
```
- frontend/ — Vue 3 SPA
- backend/ — .NET 10 API (clean-ish layers)
- cms/ — Sanity Studio
- k8s/ — Kubernetes manifests (kustomize base + overlays)
- infrastructure/ — Terraform + Ansible (provisioning + deploy)
- scripts/ — automation utilities (seed, pings, helpers)
- make/ + root Makefile — the main entry point for workflows
```
---

## Fast access to other documentation:
### Backend:

- [Backend core](https://github.com/Zyabridos/real_estate_portal/blob/main/backend/README.md)
- [Backend tests](https://github.com/Zyabridos/real_estate_portal/blob/main/backend/RealEstate.Tests/README.md)
### Frontend
- [Frontend core](https://github.com/Zyabridos/real_estate_portal/blob/main/frontend/README.md)
- [Frontend e2e tests](https://github.com/Zyabridos/real_estate_portal/blob/main/frontend/tests/e2e/README.md)
### CMS:
- [CMS](https://github.com/Zyabridos/real_estate_portal/blob/main/cms/README.md)
### Infrastructure
- [Infrastructure core](https://github.com/Zyabridos/real_estate_portal/blob/main/infrastructure/README.md)
- [Kubernetes](https://github.com/Zyabridos/real_estate_portal/blob/main/k8s/README.md)
- [Ansible](https://github.com/Zyabridos/real_estate_portal/blob/main/infrastructure/ansible/README.md)
- [Terraform](https://github.com/Zyabridos/real_estate_portal/blob/main/infrastructure/terraform/README.md)
## Local Development

### Prerequisites
- Docker + Docker Compose
- `make` (optional, but recommended)

### 1) Configure environment variables
This repo ships env templates in the root:
- `.env.dev.example` — main local dev values (Mongo credentials, DB name)
- `.env.e2e.example` — isolated values for E2E runs

```bash
cp .env.development.example .env.development
```

2) Start the stack (MongoDB + API + Frontend)
```bash
make up-d
```
URLs (default):

- Frontend: http://localhost:3000
- Backend (host): http://localhost:5055
- Backend health: http://localhost:5055/api/health/liveness
- Mongo check: http://localhost:5055/api/health/readiness

3) Start Sanity Studio (optional)

The CMS is optional and runs via Docker Compose profile:
```bash
docker compose --env-file .env.development --profile cms up -d --build
```
CMS Studio:

- http://localhost:3333

4) Seed demo data (optional)

Seed scripts expect backend URL/port:
```bash
BACKEND_PORT=5055 BACKEND_URL=http://localhost:5055 make seed
```
## Testing
### Backend
```bash
make test-backend
```
Generate coverage report:
```bash
make test-backend-coverage
```

### Frontend
Unit-tests:
```bash
make test-frontend-unit
```
E2E-tests:
```bash
make test-frontend-e2e
```

## Production: Kubernetes is the source of truth

Production runtime is Kubernetes (k3s). Manifests live in k8s/ and are managed via kustomize overlays:

- k8s/overlays/prod-blue
- k8s/overlays/prod-green

Provisioning + deployments are orchestrated by Terraform + Ansible in infrastructure/.
Typical rollout is Blue/Green:

1) deploy to green
2) verify health
3) switch traffic (Load Balancer)
4) optionally deploy the same version to blue (or keep as fallback)

One-command deploy:
```bash
make deploy-green
```
or
```bash
make deploy-blue
```
## Note about *production* Docker files

This repo contains docker-compose.production.yml and Dockerfile.production variants.
They are not used as the production runtime path (production runs on Kubernetes).
They are intentionally kept as:

- a reference for production-like container builds,
- a convenient single-node smoke/debug option outside Kubernetes,
- a fallback deployment path for environments where Kubernetes is not available.

Local development uses the non-production Dockerfiles referenced by `docker-compose.yml`.