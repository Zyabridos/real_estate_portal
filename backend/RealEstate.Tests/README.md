
# Tests Architecture

This folder contains  all automated tests  for the RealEstate backend solution.  
Tests are structured  by application layer.

## Test Projects Overview

### Integration/Endpoints

**Purpose:**  
Integration tests for HTTP API endpoints.

**What is tested:**

-   controllers (`/api/properties`, etc.)

-   routing and HTTP status codes

-   real MongoDB behavior via Testcontainers
## List of tested API scenarios

- **`GET /api/{entities}`** returns a paged contract  
  _(items + totals + paging metadata)_

- **`GET /api/{entities}/{id}`** returns:
    - **`200`** for an existing entity
    - **`404`** for a missing entity
    - **`400`** for an invalid GUID in route

- **`POST /api/{entity}`** returns:
    - **`201`** for a valid payload
    - **`400`** for an invalid payload (validation)
    - **`404`** when the referenced `propertyId` does not exist

- **Link checks**
    - A created lead is linked to an existing property  
      _(response DTO + persisted Mongo document)_

These tests use:

- `Microsoft.AspNetCore.Mvc.Testing`
- real MongoDB container
- shared fixtures from  `RealEstate.Testing`


----------

###  Infrastructure

**Purpose:**  
Tests for  infrastructure layer  (repositories, Mongo queries).

**What is tested:**
- MongoDB queries and filters
- Sorting and paging logic
- Update / delete behavior
- Links between entities (e.g. broker ↔ property, lead ↔ property)
- Mongo index initialization expectations (where applicable)
- Mapping-related persistence assumptions (document shape ↔ domain entity)
----------

### Validation

**Purpose:**  
Unit tests for validation logic.

**What is tested:**

-   FluentValidation rules

-   required fields

-   border cases

-   invalid inputs

----------

### TestData (Shared Test Infrastructure)

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
