
# RealEstate Backend API

Backend service for the Real Estate demo project.  
Built with ASP.NET Core Web API (.NET 10) and designed as a clean, testable foundation for a production-grade system.

The backend is intended to be used together with:
- a Vue 3 SPA frontend
- Sanity CMS for marketing and editorial content

---

## Tech Stack

- .NET 10 / ASP.NET Core Web API
- MongoDB (business & user-generated data)
- FluentValidation
- Swagger
- xUnit and FluentAssertions

---

## Repository Structure
```
src/
  Api/                     # Controllers, Swagger, config, bootstrap
  Application/
    Domain/                # Entities + Enums
    Features/              # Brokers/, Leads/, Properties/, etc.
    Infrastructure/        # Mongo + Repositories
    Common/                # PagedResult, normalizers, shared helpers, etc.
```
MongoDB is used for **business data only**  
(articles, blog content, guides are handled by Sanity CMS).

---

## MongoDB Configuration
The API uses the Options pattern for MongoDB configuration.

Rename the example config:
```bash
mv src/Api/appsettings.example.json src/Api/appsettings.json
```
Then adjust the values according to your local environment:
```bash
nano src/Api/appsettings.json
```

## Prerequisites

This project is designed to be developed and tested using Docker.  
Most dependencies (MongoDB, test containers, etc.) run inside containers, so Docker is the recommended setup.

- Docker (required for integration tests)

## Notes
- This project is educational but production-oriented
- Focus is on clean architecture, testability, and maintainability
- Authentication & authorization are intentionally out of scope for the initial phase
- The API is designed to be consumed by a SPA frontend
- CI runs on PRs/pushes and performs restore/build/test
- Integration tests require Docker because they use Testcontainers