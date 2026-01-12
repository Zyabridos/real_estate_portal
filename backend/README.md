
# RealEstate Backend API

Backend service for the Real Estate demo project.  
Built with ASP.NET Core Web API and designed as a clean, testable foundation for a production-grade system.

The backend is intended to be used together with:
- a **Vue 3 SPA frontend**
- **Sanity CMS** for marketing and editorial content

---

## Tech Stack

- **.NET 10 / ASP.NET Core Web API**
- **MongoDB** (business & user-generated data)
- **MongoDB.Driver**
- **Testcontainers for .NET** (integration testing)
- **xUnit + FluentAssertions**
- **Docker**


## Architecture Overview

- **Domain** — core entities and enums (no infrastructure dependencies)
- **Application** — contracts (repositories, DTOs, service abstractions)
- **Infrastructure** — MongoDB integration, repositories, indexes
- **API** — HTTP entry point, DI, configuration, controllers (later)

MongoDB is used for **business data only**  
(articles, blog content, guides are handled by Sanity CMS).


## MongoDB Configuration

The API uses the Options pattern for MongoDB configuration.

Required configuration section:

```
json
"Mongo": {
  "ConnectionString": "mongodb://localhost:27017",
  "Database": "realestate"
}
```
See appsettings.example.json for reference.

## MongoDB Conventions
Global MongoDB conventions are configured on application startup:
- camelCase field names
- Guid stored as string
- Enums stored as string

Unknown fields are ignored (forward-compatible schema)

Indexes are created automatically on startup via a hosted service.

## Prerequisites
- .NET SDK 10.0.101
- Docker (required for integration tests via Testcontainers)
- MongoDB is **not required** locally when running tests

## Running the API locally
From the `backend/` directory:

```bash
dotnet restore
dotnet run --project src/RealEstate.Api
```
The API will be available at:

```arduino
http://localhost:5000
```
(or the port defined in configuration)

## Running Tests
Integration tests use Testcontainers and start a real MongoDB container automatically.

Docker must be running.

From the `backend/` directory:

```bash
dotnet test RealEstate.slnx
```

**What is covered:**
- MongoDB integration
- Repository CRUD operations
- Filtering & paging queries
- Collection availability / smoke tests

## Running with Docker

```bash
docker build -t realestate-backend .
docker run -p 5000:5000 --env-file .env realestate-backend
```
## Notes
- This project is educational but production-oriented
- Focus is on clean architecture, testability, and maintainability
- Authentication & authorization are intentionally out of scope for the initial phase
- The API is designed to be consumed by a SPA frontend
- CI runs on PRs/pushes and initialize restore/build/test
- Integration tests require Docker because they use Testcontainers
