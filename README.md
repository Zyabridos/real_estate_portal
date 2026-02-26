

# Real Estate Mini Portal

Educational full-stack project for learning **Vue 3**, **.NET Web API (C#)**, and **Sanity CMS** in a real estate domain.

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

### Infrastructure
- Docker / Docker Compose (local dev)
- Kubernetes (k3s) + Helm (addons) + Kustomize (manifests)
- Terraform (Hetzner Cloud: servers, networking, firewall, load balancer)
- Ansible (k3s bootstrap + addons + kustomize deploy + verification)
- Makefile as a single entry point for local + production workflows
---


## Repository Structure

```
  frontend/          # Vue application (UI, routing, state management)
  backend/           # .NET Web API (business logic, database)
  cms/               # Sanity Studio (editorial content)
  infrastructure /   # Terraform + Ansible (provisioning + deployment)
 
  make/              # Makefile modules (e.g., docker targets)
  scripts/           # Utilities and automation (seed scripts, helpers)
  
  docker-compose.yml # Local dev stack (MongoDB + API + UI + optional CMS)
  Makefile           # Entry point for common dev commands
```

## Prerequisites

### For local development:
- Docker + Docker Compose

### For provisioning/deployment (infrastructure):
- Terraform
- Ansible
- kubectl + helm
- SSH access to provisioned hosts (SSH key)
- Ansible Vault password file: infrastructure/ansible/vault-password

Optional but recommended:
- `make`

## Deployment & High Availability (Blue/Green)

Production infrastructure uses **1 Load Balancer** and **2 application servers**:

- **Load Balancer:** single entry point for all traffic (HTTP/HTTPS).
- **Two servers:** `blue` and `green`, running identical stack.

### Automatic deployment on merge to `main`

Deployment is triggered **automatically on every merge to `main`** and follows a **blue/green rollout**:

1. Deploy to green server first (warm-up / health check).
2. If green is healthy, deploy to **blue** server next.

### Fault tolerance

If one server becomes unavailable (e.g., crash or failed deployment), the **other server stays online** behind the Load Balancer, so the application remains accessible.

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

## Production Deployment (Blue/Green)
Production deploy is designed as Blue/Green stacks (prod-blue, prod-green) to reduce downtime and make rollbacks safe.

High-level idea:

1) Deploy a new version to green
2) Verify health
3) Switch traffic to green (Load Balancer target selector)
4) Optionally deploy the same version to blue (or keep as fallback)

**One-command deploy (recommended)**

**Blue:**
```bash
make deploy-blue
```

**Green:**
```bash
make deploy-green
```

What it does:

1. Terraform init/apply in workspace $(ENV)-$(STACK) (defaults: prod-blue)
2. Generate Ansible inventory from Terraform outputs 
3. Refresh ~/.ssh/known_hosts 
4. Run Ansible playbook: install/upgrade k3s, fetch kubeconfig, install addons (ingress-nginx/cert-manager/metrics-server), apply kustomize overlay, verify rollout

## Notes on Configuration
- Frontend API calls are proxied via Vite (/api → backend)
- Backend communicates with MongoDB via Docker network
- MongoDB runs only once per test suite (Testcontainers)
- CMS is optional and started via Docker profiles