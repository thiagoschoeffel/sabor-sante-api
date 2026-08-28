# Sabor Santè API

REST API developed to support the operation of **Sabor Santè**, a healthy food company.

The project also serves as a progressive study of backend development with **.NET 10**, **ASP.NET Core** and **PostgreSQL**.

The proposal is to build the system starting from simple solutions and introducing new concepts, patterns, abstractions and tools only when concrete problems justify their use.

---

## Objectives

The project has two complementary objectives:

1. deepen knowledge in backend development, databases, automated testing, software architecture and distributed systems;
2. gradually evolve into a solution capable of supporting the real operation of Sabor Santè.

In the long term, the solution could also be evaluated as a foundation for other companies in the healthy food and meal production segment.

---

# Development Philosophy

The project does not start with a complex ready-made architecture.

The evolution follows a cycle similar to:

```text
Real need
    ↓
Simple implementation
    ↓
Problem appears
    ↓
Problem understanding
    ↓
Introduction of concept or pattern
    ↓
Tests
    ↓
Refactoring
    ↓
Versioning
```

Principles adopted:

- start with the simplest possible solution;
- avoid abstractions without concrete necessity;
- understand the technology before adding tools that hide its complexity;
- separate responsibilities as the system grows;
- use real business needs to guide modeling;
- preserve data integrity and history;
- use PostgreSQL also as a mechanism to guarantee integrity;
- write automated tests for relevant rules and integrations;
- evolve the system through small increments;
- avoid patterns just because they are popular in the market.

---

# Current Stack

## Application

- .NET 10
- C#
- ASP.NET Core
- Minimal APIs
- Npgsql
- PostgreSQL

## Infrastructure

- Docker
- Docker Compose

## Testing

- xUnit
- Microsoft.AspNetCore.Mvc.Testing
- `WebApplicationFactory`
- Real PostgreSQL for integration tests

## Development

- Git
- GitHub
- VS Code
- Insomnia

---

# Current Structure

```text
SaborSante/
├── README.md
├── SaborSante.slnx
├── src/
│   └── SaborSante.Api/
│       ├── database/
│       │   ├── 001_create_clientes.sql
│       │   └── 002_create_clientes_enderecos.sql
│       │
│       ├── Cliente.cs
│       ├── ClienteEndpoints.cs
│       ├── ClienteService.cs
│       ├── ClienteRepository.cs
│       ├── IClienteRepository.cs
│       │
│       ├── ClienteEndereco.cs
│       ├── ClienteEnderecoEndpoints.cs
│       ├── ClienteEnderecoService.cs
│       ├── ClienteEnderecoRepository.cs
│       ├── IClienteEnderecoRepository.cs
│       │
│       ├── Resultado.cs
│       ├── Program.cs
│       ├── docker-compose.yml
│       └── SaborSante.Api.csproj
│
└── tests/
    └── SaborSante.Api.Tests/
        ├── ClienteServiceTests.cs
        ├── ClienteEnderecoServiceTests.cs
        │
        ├── ClienteRepositoryIntegrationTests.cs
        ├── ClienteEnderecoRepositoryIntegrationTests.cs
        │
        ├── HealthEndpointTests.cs
        ├── ClienteEndpointTests.cs
        ├── ClienteEnderecoEndpointTests.cs
        │
        ├── FakeClienteRepository.cs
        ├── FakeClienteEnderecoRepository.cs
        │
        ├── PostgresFixture.cs
        ├── PostgresIntegrationTestCollection.cs
        ├── ApiWebApplicationFactory.cs
        ├── appsettings.IntegrationTests.json
        │
        └── SaborSante.Api.Tests.csproj
```

The dependency between projects follows only one direction:

```text
SaborSante.Api.Tests
        ↓
SaborSante.Api
```

The production project does not depend on the test project.

---

# Current Architecture

The application maintains a purposefully simple architecture:

```text
HTTP
 ↓
Endpoints
 ↓
Services
 ↓
Repositories
 ↓
Npgsql
 ↓
PostgreSQL
```

## Clients

```text
ClienteEndpoints
      ↓
ClienteService
      ↓
IClienteRepository
      ↑
ClienteRepository
      ↓
NpgsqlDataSource
      ↓
PostgreSQL
```

## Addresses

```text
ClienteEnderecoEndpoints
        ↓
ClienteEnderecoService
        ├── IClienteRepository
        │       ↑
        │  ClienteRepository
        │
        └── IClienteEnderecoRepository
                ↑
          ClienteEnderecoRepository
                ↓
             PostgreSQL
```

Interfaces were not created in advance.

They emerged when the services needed to be tested in isolation and the direct dependency on concrete repositories began to hinder testing.

---

# Technical Evolution of the Project

## 1 — API Foundations

The project started with ASP.NET Core and data stored in memory.

Studied:

- `Program.cs`;
- top-level statements;
- `WebApplicationBuilder`;
- `WebApplication`;
- Kestrel;
- middleware;
- Minimal APIs;
- routing;
- route parameters;
- model binding;
- lambdas;
- records;
- LINQ;
- JSON;
- REST;
- HTTP status codes;
- CRUD.

First endpoints:

```text
GET    /health
GET    /clientes
GET    /clientes/{id}
POST   /clientes
PUT    /clientes/{id}
DELETE /clientes/{id}
```

---

## 2 — Persistence with PostgreSQL

In-memory storage was replaced with PostgreSQL running in Docker.

Studied:

- PostgreSQL;
- Docker;
- Docker Compose;
- volumes;
- connection strings;
- Npgsql;
- manual SQL;
- parameterized SQL;
- SQL Injection;
- database-generated identity;
- `INSERT ... RETURNING`;
- manual result reading;
- SQL → C# mapping;
- `ExecuteScalarAsync`;
- `ExecuteReaderAsync`;
- `ExecuteNonQueryAsync`;
- `async`;
- `await`;
- `using`;
- `await using`;
- connection pooling.

---

## 3 — Separation of Concerns

As `Program.cs` grew, the application was gradually separated into:

```text
Program
  ↓
Endpoints
  ↓
Services
  ↓
Repositories
```

Studied:

- extension methods;
- Dependency Injection;
- constructor dependencies;
- Singleton;
- Scoped;
- Transient;
- `NpgsqlDataSource`;
- Repository;
- Service;
- separation between HTTP, application rules and persistence.

---

## 4 — Validation and Normalization

The first application rules were introduced in `ClienteService`.

Current client model:

```csharp
public record Cliente(
    int Id,
    string Nome,
    string Telefone
);
```

Requests:

```csharp
public record CriarClienteRequest(
    string? Nome,
    string? Telefone
);

public record AtualizarClienteRequest(
    string? Nome,
    string? Telefone
);
```

Current rules include:

- name required;
- phone required;
- removal of unnecessary spaces;
- phone stored only with digits;
- handling of `null` values.

Example:

```text
Input
(47) 99999-9999

↓ normalization

47999999999
```

Flow:

```text
External input
      ↓
Normalization
      ↓
Validation
      ↓
Persistence
```

---

## 5 — Result Pattern

Expected application errors are now represented explicitly through:

```text
Resultado<T>
```

Current error types:

```text
Validacao
NaoEncontrado
Conflito
```

Translation to HTTP:

```text
Validacao
→ 400 Bad Request

NaoEncontrado
→ 404 Not Found

Conflito
→ 409 Conflict
```

Studied:

- generics;
- records with behavior;
- factory methods;
- enums;
- switch expressions;
- expected errors;
- unexpected exceptions;
- separation between application result and HTTP protocol.

---

## 6 — Integrity and Concurrency

Phone uniqueness no longer depends only on the application.

The rule also started to be guaranteed by PostgreSQL.

Studied:

- constraints;
- unique indexes;
- race conditions;
- `PostgresException`;
- SQLSTATE;
- `23505 - unique_violation`;
- translation of persistence errors.

---

## 7 — Soft Delete of Clients

Clients are now inactivated instead of physically removed.

In PostgreSQL:

```sql
ativo BOOLEAN NOT NULL DEFAULT TRUE
```

Logical deletion uses:

```sql
UPDATE clientes
SET ativo = FALSE
WHERE id = @id
  AND ativo = TRUE;
```

The `ativo` field exists in persistence, but is not part of the public `Cliente` record.

Studied:

- soft delete;
- historical preservation;
- lifecycle;
- business state;
- difference between HTTP `DELETE` and physical deletion.

---

## 8 — Partial Phone Uniqueness

The current rule is:

> Phone must be unique among active clients.

Implementation:

```sql
CREATE UNIQUE INDEX ux_clientes_telefone_ativo
ON clientes (telefone)
WHERE ativo = TRUE;
```

So:

```text
inactive client with phone X
+
new active client with phone X
→ allowed
```

but:

```text
two active clients with phone X
→ conflict
```

---

## 9 — Client Reactivation

Endpoint:

```text
PATCH /clientes/{id}/reativar
```

States handled:

```text
Reativado
NaoEncontrado
JaAtivo
Conflito
```

Possible responses:

```text
client reactivated
→ 204 No Content

client not found
→ 404 Not Found

client already active
→ 409 Conflict

phone used by another active client
→ 409 Conflict
```

---

## 10 — Database Versioning

PostgreSQL structure is now represented by versioned SQL scripts in Git:

```text
database/
├── 001_create_clientes.sql
└── 002_create_clientes_enderecos.sql
```

There is still no automated migrations tool.

The intention is to understand the problem first before introducing a tool to solve it.

---

# Client Addresses

## Relationship

The first real domain relationship was introduced:

```text
Client
1 ───────── N
        Addresses
```

Example:

```text
Client
├── Home
├── Work
└── Other
```

This represents a real operational need: the same client can receive orders at different addresses.

In the database:

```text
clientes.id
     ↑
     │ Foreign Key
     │
clientes_enderecos.cliente_id
```

---

## Address Model

```text
ClienteEndereco
├── Id
├── ClienteId
├── Identificacao
├── Logradouro
├── Numero
├── Complemento
├── Bairro
├── Cidade
├── Cep
└── Ativo
```

Studied:

- one-to-many relationship;
- foreign keys;
- referential integrity;
- parent and child resources;
- nested routes;
- `DBNull.Value`;
- `reader.IsDBNull`;
- ownership.

---

## Address Endpoints

```text
GET    /clientes/{clienteId}/enderecos
GET    /clientes/{clienteId}/enderecos/{enderecoId}

POST   /clientes/{clienteId}/enderecos

PUT    /clientes/{clienteId}/enderecos/{enderecoId}

DELETE /clientes/{clienteId}/enderecos/{enderecoId}

PATCH  /clientes/{clienteId}/enderecos/{enderecoId}/reativar
```

Rules:

- client must exist;
- address must belong to the informed client;
- addresses have soft delete;
- inactive addresses do not appear in normal queries;
- addresses can be reactivated;
- required fields are validated;
- empty optional fields are normalized to `null`.

Example of ownership:

```text
address 10 belongs to client 2

GET /clientes/2/enderecos/10
→ 200 OK

GET /clientes/3/enderecos/10
→ 404 Not Found
```

---

# Automated Tests

The current strategy has three main levels.

```text
                 HTTP
                  │
           API Tests
                  │
                  ▼
              Endpoints
                  │
                  ▼
              Services
             ▲        │
             │        ▼
       Unit tests   Repositories
          │           ▲
        Fakes         │
                      │
           Integration tests
                      │
                      ▼
                 PostgreSQL
```

Each level answers different questions.

---

# Unit Tests

Services are tested in isolation using manual fakes.

## ClienteService

```text
ClienteService
      ↓
IClienteRepository
      ↑
FakeClienteRepository
```

## ClienteEnderecoService

```text
ClienteEnderecoService
        ├── FakeClienteRepository
        └── FakeClienteEnderecoRepository
```

The fakes allow:

- configure results;
- simulate existing and non-existing resources;
- capture arguments;
- count calls;
- verify if a dependency was used;
- run tests without PostgreSQL.

Studied:

- xUnit;
- `[Fact]`;
- Arrange / Act / Assert;
- test doubles;
- manual fake;
- `Task.FromResult`;
- object initializer;
- state verification;
- interaction verification;
- sent arguments;
- number of calls.

---

## ClienteService Coverage

### Creation

Tests for:

- name required;
- phone required;
- `null` values;
- phone without digits;
- normalization;
- success;
- conflict;
- repository not called on invalid validations;
- arguments sent to repository.

### Update

Tests for:

- success;
- client not found;
- conflict;
- normalization;
- validation;
- repository calls.

### Deletion

Tests for:

- success;
- client not deleted;
- ID sent;
- number of calls.

### Reactivation

Tests for:

- reactivated;
- not found;
- already active;
- conflict;
- ID sent;
- number of calls.

---

## ClienteEnderecoService Coverage

### Creation

Tests for:

- client not found;
- success;
- normalization;
- optional fields;
- validation of required fields;
- repository not called in invalid scenarios.

### Update

Tests for:

- success;
- address not found;
- client not found;
- normalization;
- optional fields;
- validation;
- arguments sent.

### Deletion

Tests for:

- success;
- address not found;
- client not found;
- IDs sent;
- repository calls.

### Reactivation

Current states:

```text
Reativado
NaoEncontrado
JaAtivo
```

Tests for:

- success;
- address not found;
- address already active;
- client not found;
- IDs sent;
- number of calls.

---

# Interfaces and Dependency Inversion

Interfaces emerged during testing.

Initially:

```text
ClienteService
      ↓
ClienteRepository
```

When trying to isolate the service, the concrete dependency made testing difficult.

The structure evolved to:

```text
ClienteService
      ↓
IClienteRepository
      ↑
ClienteRepository
```

and:

```text
ClienteEnderecoService
        ↓
IClienteEnderecoRepository
        ↑
ClienteEnderecoRepository
```

This introduced in practice:

- abstractions;
- contracts;
- replacement of implementations;
- reduced coupling;
- Dependency Injection through interfaces;
- dependency inversion.

The abstraction was not created in advance: it emerged from a real testability problem.

---

# Integration Tests with PostgreSQL

Unit tests can prove service rules, but cannot prove that this works:

```text
Repository
↓
Npgsql
↓
SQL
↓
PostgreSQL
```

For example, a fake would not necessarily discover:

- wrong table name;
- wrong column;
- invalid SQL;
- error in `RETURNING`;
- incompatible PostgreSQL type;
- incorrect foreign key;
- missing index.

A separate database was created:

```text
saborsante_tests
```

The development database remains:

```text
saborsante
```

So:

```text
Development
→ saborsante

Tests
→ saborsante_tests
```

---

## PostgresFixture

The PostgreSQL infrastructure used by tests was centralized in:

```text
PostgresFixture
```

It is responsible for:

- loading test configuration;
- creating the `NpgsqlDataSource`;
- sharing the datasource;
- disposing the resource at the end;
- providing repositories;
- cleaning the database between tests.

Studied:

- `IClassFixture`;
- `IAsyncLifetime`;
- lifecycle of test resources;
- sharing infrastructure;
- data preparation and cleanup.

---

## Cleanup and Isolation

Initially each test had a `finally` that manually deleted the created data.

This generated a lot of repetition.

The strategy evolved to centralized cleanup:

```text
LimparBancoAsync()
↓
DELETE clientes_enderecos
↓
DELETE clientes
```

Each test starts with a known state.

---

## Concurrency Between Tests

When running different integration test classes simultaneously, a race condition appeared.

Example:

```text
Test A
↓
creates client

              Test B
              ↓
              cleans database

Test A continues
↓
client disappeared
```

Conflicts with foreign keys also occurred during concurrent cleanups.

The problem appeared because multiple classes used the same mutable database:

```text
saborsante_tests
```

The solution was to group these tests in an xUnit collection:

```text
Postgres Integration
```

with parallelization disabled.

So:

```text
Test A
↓
finishes

Test B
↓
finishes

Test C
↓
finishes
```

instead of:

```text
Test A ────────┐
                ├── same database
Test B ────────┘
```

Concepts studied:

- isolation;
- shared state;
- concurrency;
- race conditions;
- foreign key violations;
- xUnit collections;
- `DisableParallelization`.

---

# Repository Integration Tests

## ClienteRepository

Tested against real PostgreSQL:

- `CriarAsync`;
- creation with duplicate phone;
- `ObterPorIdAsync`;
- non-existent client;
- `AtualizarAsync`;
- non-existent client;
- phone conflict;
- `ExcluirAsync`;
- soft delete;
- non-existent deletion;
- `ReativarAsync`;
- not found;
- already active;
- phone conflict.

These tests really exercise:

```text
C#
↓
Npgsql
↓
SQL
↓
constraints
↓
indexes
↓
PostgreSQL
```

---

## ClienteEnderecoRepository

Tested:

- creation;
- read by ID;
- listing;
- update;
- deletion;
- reactivation;
- soft delete;
- inactive records;
- non-existent address;
- ownership;
- relationship with client;
- foreign key.

Example:

```text
client 1 owns address 10

client 2 tries to update address 10
↓
false
```

So the ownership rule is also verified directly in persistence.

---

# Automated HTTP API Tests

After unit tests and repository tests there was still a gap:

```text
HTTP
↓
?
↓
Service
↓
Repository
```

It was necessary to test the application from its external boundary.

Introduced:

```text
Microsoft.AspNetCore.Mvc.Testing
```

and:

```text
WebApplicationFactory<Program>
```

Now tests can execute:

```text
HttpClient
↓
ASP.NET Core
↓
Routing
↓
Model Binding
↓
Endpoints
↓
Services
↓
Repositories
↓
Npgsql
↓
PostgreSQL
↓
HTTP Response
```

without needing to manually start the application or use Insomnia.

---

# HealthEndpointTests

The first automated HTTP test was:

```text
GET /health
```

It verifies:

- application initializes;
- route exists;
- response is exactly `200 OK`;
- response is JSON;
- `status` property;
- value `"ok"`.

Used:

- `HttpClient`;
- `HttpStatusCode`;
- `GetAsync`;
- `ReadFromJsonAsync<T>`.

---

# ApiWebApplicationFactory

Created:

```text
ApiWebApplicationFactory
```

deriving from:

```csharp
WebApplicationFactory<Program>
```

It represents the application used by HTTP tests.

An important problem emerged during its implementation.

Simply adding:

```text
appsettings.IntegrationTests.json
```

to the configuration was not enough to make the application use `saborsante_tests`.

The application still had the `NpgsqlDataSource` configured by `Program.cs`.

The problem became clear when:

```text
PostgresFixture
→ cleaned saborsante_tests

API
→ used saborsante
```

A test that should return:

```text
201 Created
```

returned:

```text
409 Conflict
```

because the phone already existed in the development database.

The fix was to explicitly replace the dependency in the test container:

```text
RemoveAll<NpgsqlDataSource>
↓
register NpgsqlDataSource from saborsante_tests
```

The structure became:

```text
PostgresFixture
        ↓
saborsante_tests
        ↑
ApiWebApplicationFactory
```

Studied:

- `WebApplicationFactory`;
- ASP.NET Core test host;
- application configuration in tests;
- `ConfigureTestServices`;
- dependency replacement;
- `RemoveAll<T>`;
- difference between configuration and instance already registered in DI.

---

# HTTP Client Tests

## POST /clientes

Verified:

```text
201 Created
400 Bad Request
409 Conflict
```

Also:

- returned JSON;
- generated ID;
- name;
- phone normalization;
- uniqueness conflict.

---

## GET /clientes/{id}

Verified:

```text
existing client
→ 200 OK

non-existent client
→ 404 Not Found
```

The JSON response is also deserialized and compared.

---

## PUT /clientes/{id}

Verified:

- valid update;
- `204 No Content`;
- persistence confirmed through `GET`;
- non-existent client;
- `404 Not Found`;
- phone conflict;
- `409 Conflict`.

---

## DELETE /clientes/{id}

Verified:

```text
existing client
→ 204 No Content

subsequent GET
→ 404 Not Found
```

confirming soft delete.

Also tested:

```text
non-existent client
→ 404 Not Found
```

---

## PATCH /clientes/{id}/reativar

Tested:

```text
inactive client
→ 204 No Content

non-existent client
→ 404 Not Found

already active client
→ 409 Conflict

phone used by another active client
→ 409 Conflict
```

The conflict scenario traverses the entire stack:

```text
HTTP
↓
Service
↓
Repository
↓
Partial Unique Index
↓
unique_violation
↓
409 Conflict
```

---

# HTTP Address Tests

## POST /clientes/{clienteId}/enderecos

Tested:

- valid creation;
- `201 Created`;
- returned JSON;
- correct `clienteId`;
- address fields;
- non-existent client;
- `404 Not Found`;
- invalid data;
- `400 Bad Request`.

---

## GET /clientes/{clienteId}/enderecos/{enderecoId}

Tested:

```text
existing address
→ 200 OK

non-existent address
→ 404 Not Found

address belongs to another client
→ 404 Not Found
```

---

## GET /clientes/{clienteId}/enderecos

The listing verifies that only addresses belonging to the informed client are returned.

Example:

```text
client 1
├── address A
└── address B

client 2
└── address C

GET /clientes/1/enderecos
→ A + B
```

---

## PUT /clientes/{clienteId}/enderecos/{enderecoId}

Tested:

- valid update;
- `204 No Content`;
- persistence confirmed by `GET`;
- non-existent address;
- incorrect ownership;
- `404 Not Found`.

---

## DELETE /clientes/{clienteId}/enderecos/{enderecoId}

Tested:

- soft delete;
- `204 No Content`;
- address no longer found;
- non-existent address;
- address of another client;
- `404 Not Found`.

---

## PATCH /clientes/{clienteId}/enderecos/{enderecoId}/reativar

Tested:

```text
inactive address
→ 204 No Content

already active address
→ 409 Conflict

non-existent address
→ 404 Not Found

non-existent client
→ 404 Not Found
```

---

# Current Test Pyramid

At this point the project has three complementary levels.

## Unit

```text
Service
↓
Fake
```

Goal:

> verify rules in isolation.

Advantages:

- fast;
- deterministic;
- no external infrastructure.

---

## Persistence Integration

```text
Repository
↓
Npgsql
↓
PostgreSQL
```

Goal:

> verify SQL, constraints, relationships and real persistence.

---

## HTTP Integration

```text
HttpClient
↓
ASP.NET Core
↓
Endpoint
↓
Service
↓
Repository
↓
PostgreSQL
```

Goal:

> verify the integrated application from the HTTP boundary.

The levels do not replace each other.

An HTTP test does not need to reproduce all cases already covered by unit tests, and a unit test cannot guarantee that SQL and PostgreSQL are correct.

---

# Concepts of Testing Studied So Far

- xUnit;
- `[Fact]`;
- Arrange / Act / Assert;
- unit tests;
- integration tests;
- HTTP tests;
- test doubles;
- fakes;
- configurable state;
- interaction;
- argument capture;
- number of calls;
- `Task.FromResult`;
- fixtures;
- `IClassFixture`;
- `IAsyncLifetime`;
- xUnit collections;
- parallelization;
- race conditions;
- test isolation;
- shared state;
- exclusive test database;
- data preparation and cleanup;
- `WebApplicationFactory`;
- `HttpClient`;
- `PostAsJsonAsync`;
- `PutAsJsonAsync`;
- `GetAsync`;
- `DeleteAsync`;
- `HttpRequestMessage`;
- `HttpMethod.Patch`;
- `ReadFromJsonAsync`;
- HTTP status;
- JSON contract;
- dependency replacement in DI.

No external mocking library has been necessary so far.

---

# Current Domain Model

```text
Cliente
   │
   └── Endereços
```

Still a small domain, but already has:

- identity;
- active/inactive state;
- lifecycle;
- uniqueness rules;
- relationship;
- ownership;
- history preserved by soft delete;
- referential integrity.

---

# Current Rules

## Client

- name required;
- phone required;
- phone normalized;
- phone unique among active clients;
- client can be inactivated;
- client can be reactivated;
- reactivation may cause phone conflict.

## Address

- belongs to exactly one client;
- client must exist;
- identification required;
- street required;
- number required;
- neighborhood required;
- city required;
- complement optional;
- postal code optional;
- empty optional fields are converted to `null`;
- address has soft delete;
- address can be reactivated;
- client address cannot be manipulated through another client.

---

# What Has Not Yet Been Introduced

The project still does not formally use:

- Clean Architecture;
- DDD;
- CQRS;
- MediatR;
- microservices;
- Event Sourcing;
- messaging;
- Entity Framework Core;
- mocking library;
- Testcontainers;
- automated migrations;
- authentication;
- authorization.

This is intentional.

These concepts will be evaluated when concrete problems justify their use.

---

# Next Step — Orders

The basic API infrastructure already has:

```text
HTTP
✓

Services
✓

Repositories
✓

PostgreSQL
✓

Unit tests
✓

Integration tests
✓

HTTP tests
✓
```

The next focus returns to Sabor Santè domain.

The next area will be:

```text
Order
```

The order begins to connect entities that today are still relatively independent:

```text
Cliente
   ↓
Endereço
   ↓
Pedido
   ↓
Itens
```

This step should raise important domain questions.

---

## Historical Address of the Order

Example:

```text
Today:
Order #100
→ Street A, 123

Tomorrow:
client changes registration
→ Street B, 500
```

The old order should still represent:

```text
Street A, 123
```

because that was the address used when the order was placed.

This will require deciding how to preserve a historical snapshot of the address.

---

## Order Status

It will be necessary to study possible states, such as:

```text
Received
↓
Confirmed
↓
In production
↓
Packaged
↓
Out for delivery
↓
Delivered
```

Questions will also arise such as:

- can order be changed after confirmed?
- can it be canceled?
- when can it no longer be changed?
- how to record state transitions?

---

## Order Items

The order should also represent:

- meals;
- quantities;
- meal types;
- extras;
- proteins;
- fruits;
- salads;
- personalizations;
- substitutions;
- plans.

---

# Future Domain Evolution

The long-term vision is:

```text
Cliente
   ↓
Endereço
   ↓
Cardápio
   ↓
Plano / Pedido
   ↓
Itens / Refeições
   ↓
Produção
   ↓
Necessidade agregada
   ↓
Embalagem
   ↓
Separação
   ↓
Entrega
   ↓
Roteirização
```

---

## Production

Individual orders should generate aggregated needs for the kitchen.

Example:

```text
João
→ 5 meals with chicken

Maria
→ 3 meals with chicken

Pedro
→ 4 meals with chicken
```

Production:

```text
Chicken
→ 12 portions
```

This step should introduce problems involving:

- aggregation;
- queries;
- dates;
- quantities;
- states;
- consistency;
- transactions;
- reports.

---

## Packaging and Separation

After production it will be necessary to represent:

```text
production completed
↓
packaging
↓
identification
↓
separation by client
↓
separation by delivery
```

---

## Delivery

The delivery operation should consider:

- chosen address;
- date;
- time;
- delivery person;
- status;
- delivery grouping.

---

## Routing

A later stage may study delivery route optimization.

This problem will only be addressed when there is sufficient data and flows to justify it.

---

# Possible Future Technical Evolutions

Beyond domain evolution, future problems may justify the study of:

- migrations;
- Entity Framework Core;
- transactions;
- authentication;
- authorization;
- structured logging;
- global error handling;
- observability;
- caching;
- background jobs;
- messaging;
- concurrency;
- performance;
- CI/CD;
- deployment;
- containers for testing;
- Testcontainers;
- more advanced software architecture.

The order is not fixed.

The real problem encountered during development will continue to determine which concept will be studied next.

---

# Project Vision

Sabor Santè involves much more than CRUD.

The operation has challenges related to:

- clients;
- multiple addresses;
- weekly menus;
- traditional meals;
- low carb meals;
- vegetarian meals;
- salads;
- proteins;
- fruits;
- extras;
- plans;
- personalizations;
- substitutions;
- orders;
- production;
- daily capacity;
- cutoff times;
- packaging;
- separation;
- delivery persons;
- deliveries;
- routing.

The intention is to model these areas progressively, as each problem begins to exist concretely in the system.

---

# Current Status

At this point the project is no longer just a basic CRUD.

It has:

```text
REST API
+
PostgreSQL
+
Manual SQL
+
Dependency Injection
+
Services
+
Repositories
+
Interfaces
+
Result Pattern
+
Validation
+
Normalization
+
Soft Delete
+
Reactivation
+
Partial Unique Index
+
Relationships
+
Foreign Keys
+
Ownership
+
Unit Tests
+
Fakes
+
Integration Tests
+
Exclusive test database
+
Fixtures
+
Isolation
+
Concurrency control
+
Automated HTTP tests
+
WebApplicationFactory
```

The next goal is to use this foundation to begin modeling more important parts of the operation domain, starting with **Orders**.
