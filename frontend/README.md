
# Real Estate Frontend

Frontend application for the Real Estate Portal project.

This SPA is responsible for rendering the user interface, managing client-side routing and state, communicating with the backend API, and displaying editorial content from Sanity CMS.

The frontend is intentionally designed with a clean, scalable architecture, suitable for production-grade applications.


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
```
src/
  app/                # App bootstrap and global setup
  layouts/            # Application layouts (AppLayout)
  pages/              # Route-level pages
  router/             # Vue Router configuration
  stores/             # Pinia stores (properties, brokers)
  shared/
    api/              # HTTP client and domain API modules
    cms/              # Sanity client (scaffold)
    config/           # Environment configuration
    ui/               # UI state components (Loading, Error, Empty)
  assets/             # Static assets
  styles/             # Global styles` 
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