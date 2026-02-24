
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


## Prerequisites

This project is designed to be developed and tested using Docker.  
Most dependencies (MongoDB, test containers, etc.) run inside containers, so Docker is the recommended setup.

- Docker (required for integration tests)

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

## Dependency Injection conventions (Scrutor scanning)

### Naming rules (must follow)

To be discovered automatically, types must follow these conventions:

1. Services

- Implementation class must end with Service (e.g. PropertyService)
- Interface must be named exactly I{ClassName} (e.g. IPropertyService)

2. Repositories

- Implementation class must end with Repository (e.g. PropertyRepository)
- Interface must be named exactly I{ClassName} (e.g. IPropertyRepository)

3. Visibility

- Types must be public and non-abstract

### Assembly anchors (important)

FluentValidation and AutoMapper need a reference point to know which assembly to scan at startup.
Currently used `PropertyService` as that reference (an “assembly anchor”) to register all validators and mapping profiles from the Application layer.

**Rule:** keep `PropertyService` (and the core mapping profile(s), e.g. `PropertyMappingProfile`) stable and “always present”.  
If you rename/move/remove these types or relocate the Properties feature to another project, update the anchor accordingly — otherwise validators/profiles may stop being discovered and you’ll see runtime DI or mapping errors.

## Notes
- This project is educational but production-oriented
- Focus is on clean architecture, testability, and maintainability
- Authentication & authorization are intentionally out of scope for the initial phase
- The API is designed to be consumed by a SPA frontend
- CI runs on PRs/pushes and performs restore/build/test
- Integration tests require Docker because they use Testcontainers