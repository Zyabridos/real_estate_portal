
# E2E tests (Playwright)

This folder contains end-to-end tests for the frontend using Playwright.

## Prerequisites

- Node.js (this app was developed under v22.21.1)
- Frontend app running locally
- Backend API running locally
- Seeded data (so catalog pages have content)
- Docker running (optional)

## Quick start with Docker

If you don’t want to run frontend/backend manually, you can start everything with Docker (from the project root):
```bash
make up-d    (Start all services (background))
```

After services are up, seed the backend so the catalog has content (example):

```bash
make seed-brokers
make seed-properties
```
Then run e2e tests:
```bash
cd frontend/tests/e2e
npm ci
npm test
```

## Project structure
```
tests/
	e2e/
	fixtures/ # routes, testIds and other shared constants
	helpers/ # waiters and test helpers
	tests/ # specs (*.spec.ts)
	types/ # predefined enums and entities
playwright.config.ts
package.json
.env.example
```

## Install

From `frontend/tests/e2e`:

```bash
npm ci
```
### Environment variables
Rename the env file and adjust:
```
mv .env.example .env
```
### Supported variables:

- E2E_BASE_URL — frontend URL (default: http://localhost:3000)

#### Example:
```bash
E2E_BASE_URL=http://localhost:3000
```
## Run tests
Run all tests (headless)
```bash
npm test
```
Run a single test file
```bash
npx playwright test tests/propertiesList.spec.ts
```
Run a single test by name
```bash
npx playwright test -g "properties filter by type"
```
### Reports & artifacts
After a run, Playwright generates an HTML report.

Open it with:
```bash
npx playwright show-report
```
If you want screenshots/traces on failures, check playwright.config.ts and enable screenshot, trace, and/or video (they *might* be disabled).

## Data seeding
These tests assume the backend has data (brokers + properties).
Run your project’s seed commands before running e2e (example from root folder):
```bash
make seed-brokers
make seed-properties
```