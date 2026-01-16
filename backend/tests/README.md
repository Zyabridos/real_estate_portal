
# Tests Architecture

This folder contains  all automated tests  for the RealEstate backend solution.  
Tests are structured  by application layer.

## Test Projects Overview

### RealEstate.Api.Tests

**Purpose:**  
Integration tests for HTTP API endpoints.

**What is tested:**

-   controllers (`/api/properties`, etc.)

-   routing and HTTP status codes

-   real MongoDB behavior via Testcontainers


**List of tested endpoints:**

-   `GET /` returning list of entities
-   `GET /{id}` 200 and entity
-   `GET /{non_existing_id}`  returning 404
-   `GET /{invalid_id}`  returning 400
-   `PUT /{id}`  returning 200 and updates entitiy
-   `DELETE  /{id}` returning 204 and then 404 on `GET /{id}`


These tests use:

- `Microsoft.AspNetCore.Mvc.Testing`
- real MongoDB container
- shared fixtures from  `RealEstate.Testing`


----------

###  RealEstate.Infrastructure.Tests

**Purpose:**  
Tests for  infrastructure layer  (repositories, Mongo queries).

**What is tested:**

-   MongoDB queries and filters

-   sorting and paging logic

-   update / delete behavior

-   links between entities (for example, broker and property)

-   mapping between Mongo documents and domain entities
----------

### RealEstate.Validation.Tests

**Purpose:**  
Unit tests for validation logic.

**What is tested:**

-   FluentValidation rules

-   required fields

-   border cases

-   invalid inputs

----------

### RealEstate.Testing (Shared Test Infrastructure)

**Purpose:**  
Shared testing utilities used by all test projects.

This project contains  no tests, only reusable code.

#### Fixtures

-   `MongoDbFixture`

    -   starts  one MongoDB Testcontainer

    -   provides  `IMongoClient`  and unique databases

-   `MongoDbTestBase`

    -   base class for Mongo-based tests

    -   ensures clean database per test class

#### TestData

-   factory helpers for test entities

-   consistent test object creation

-   avoids copy-paste in tests


Example:
```bash
var property = TestProperties.Create(
    city: "Trondheim",
    type: PropertyType.Apartment,
    price: 4_000_000m
);
```` 

----------

## MongoDB Strategy

-   One MongoDB container  per test run

-   One database per test class

-   Database is cleared before each test class

-   Safe for parallel execution

## Running Tests

Run all tests:

```csharp
dotnet test
```

Run a specific project:
```csharp
dotnet test tests/RealEstate.Api.Tests
```
```csharp
dotnet test tests/RealEstate.Infrastructure.Tests
```
```csharp
dotnet test tests/RealEstate.Validation.Tests
```
