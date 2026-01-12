

# Real Estate Mini Portal

Educational full-stack project for learning **Vue 3**, **.NET Web API (C#)**, and **Sanity CMS** in a real estate domain.

The project is inspired by data-driven listing systems used for managing property inventories, brokers, and customer inquiries.

---

## Badges
[![Backend Integration Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-integration-tests.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-integration-tests.yml)
[![Backend Unit Tests](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-unit-tests.yml/badge.svg)](https://github.com/Zyabridos/real_estate_portal/actions/workflows/backend-unit-tests.yml)


## Tech Stack

### Frontend
- Vue 3
- Vue Router
- Pinia
- Tailwind CSS
- TypeScript

### Backend
- .NET 10 (ASP.NET Core Web API)
- Entity Framework Core
- MongoDB

### CMS
- Sanity v3
- GROQ
- Portable Text

---

## Project Structure

```
  frontend/   # Vue application (UI, routing, state management)
  backend/    # .NET Web API (business logic, database)
  cms/        # Sanity Studio (editorial content)
  
  docker-compose.yml
  Makefile
  LICENSE
```
## Environment Variables

Rename `.env` file:
```bash  
mv .env.example .env
```  
Then set correct values to keys.


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

----------

## Notes on Configuration

-   Frontend API requests are routed through a Vite development proxy (`/api → backend`).
-   Backend services communicate with MongoDB via the Docker network.
-   CMS is optional and can be started separately using Docker profiles.
