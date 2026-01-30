
# Real Estate Frontend

Frontend application for the Real Estate Portal project.

This SPA is responsible for rendering the user interface, managing client-side routing and state, communicating with the backend API, and displaying editorial content from Sanity CMS.


## Tech Stack

-   **Vue 3**

-   **TypeScript**

-   **Vue Router**

-   **Pinia**

-   **Tailwind CSS**

-   **Vite**
- **Sanity CMS**


## Responsibilities

-   Render property listings and property details

-   Display brokers and broker details

-   Handle routing and page navigation

-   Manage UI state (loading, empty, error)

-   Fetch data from backend API via a unified API client

-   Display blog content from Sanity CMS (scaffolded)

-   Provide a solid foundation for E2E testing (Playwright)



## Architecture Overview

The frontend follows a layered structure:

-   **Pages** – route-level views

-   **Stores** – state management (Pinia)

-   **Shared** – reusable infrastructure (API, config, UI states)

-   **Layout** – application shell and navigation


Key architectural principles:

-   No direct HTTP calls from pages (API layer only)

-   Unified error handling (`ApiError`)

-   Predictable UI states across all pages

-   Clear separation between domain data and UI concerns



## Project Structure

```bash
frontend/
  src/
    app/              # App bootstrap and global setup
    features/         # Feature modules (brokers, leads, properties, etc.)
    entities/         # Domain entities and helpers
    shared/           # API client, config, shared UI/components, utils
    assets/           # Static assets
    RealEstate.*      # Project-specific API/contracts modules
  tests/
    unit/             # Unit tests (Vitest)
    e2e/              # E2E tests (Playwright)
 ```
## Routing

Configured routes:

-   `/properties` – property list

-   `/properties/:id` – property details

-   `/brokers` – broker list

-   `/brokers/:id` – broker details

-   `/blog` – blog list (Sanity)

-   `/blog/:slug` – blog article (Sanity)

-   `*` – Not Found (404)


----------

## Environment Variables

Rename `.env` file:
```bash
(from the ./frontend folder)
mv .env.example .env
```
Then set correct values to keys.

## Development

Install dependencies:

`npm install`

Start the development server:

`npm run dev`

The application will be available at:

`http://localhost:3000`


## Build

Create a production build:

`npm run build`

Preview the production build locally:

`npm run preview`

----------

## Notes

-   Authentication is intentionally not implemented (possible feature in future)

-   All filtering, pagination, and sorting are handled by the backend

-   Sanity CMS integration is scaffolded and expanded in later PRs

-   Unit tests are using Vitest and E2E tests are using Playwright

-   This project prioritizes **architecture, clarity, and testability** over feature completeness