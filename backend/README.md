
# RealEstate Backend API

Backend service for the **Real Estate / Broker** demo project.  
Built with **ASP.NET Core Web API** and intended to be used together with a Vue frontend and Sanity CMS.

----------

## Tech Stack

-   **.NET (ASP.NET Core Web API)**

-   **Entity Framework Core**

-   **Docker** (for local development / deployment)


----------

## Project Structure
```
backend/
├── RealEstate.Api/
│   ├── Program.cs
│   ├── Properties/
│   ├── appsettings.json
│   ├── appsettings.example.json
│   └── RealEstate.Api.csproj
├── .env.example
├── Dockerfile
└── RealEstate.slnx` 
```
----------

## Prerequisites

-   .NET SDK (developed with **10.0.101** version)
-   Docker (optional)
-   Eventually database


----------

## Running the API locally

From the `backend/RealEstate.Api` directory:

```
dotnet restore
dotnet run
```

The API will be available at:

```
http://localhost:5000 (or the port defined in configuration)
```

----------

## Running with Docker

From the `backend/` directory:

```
docker build -t realestate-backend .
docker run -p 5000:5000 --env-file .env realestate-backend
```

----------

## API Responsibilities

This API handles **business and user-generated data**, including:

-   Properties (real estate objects)

-   Brokers (real estate agents)

Content such as blog articles and guides is handled separately by **Sanity CMS**.

----------

## Notes

-   This project is intentionally simple and educational.

-   Authentication and authorization are **out of scope** for the initial version.

-   The API is designed to be consumed by a SPA frontend.
