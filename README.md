# Sabor Santè API

API REST desenvolvida para apoiar a operação da **Sabor Santè**, uma empresa de alimentação saudável.

O projeto também funciona como um estudo progressivo de desenvolvimento backend com **.NET 10**, **ASP.NET Core** e **PostgreSQL**.

A proposta é construir o sistema partindo de soluções simples e introduzir novos conceitos, padrões, abstrações e ferramentas somente quando problemas concretos justificarem sua utilização.

---

## Objetivos

O projeto possui dois objetivos complementares:

1. aprofundar conhecimentos em desenvolvimento backend, bancos de dados, testes automatizados, arquitetura de software e sistemas distribuídos;
2. evoluir gradualmente para uma solução capaz de apoiar a operação real da Sabor Santè.

No longo prazo, a solução também poderá ser avaliada como base para outras empresas do segmento de alimentação saudável e produção de refeições.

---

# Filosofia de desenvolvimento

O projeto não começa com uma arquitetura complexa pronta.

A evolução segue um ciclo semelhante a:

```text
Necessidade real
      ↓
Implementação simples
      ↓
Problema aparece
      ↓
Entendimento do problema
      ↓
Introdução de conceito ou padrão
      ↓
Testes
      ↓
Refatoração
      ↓
Versionamento
```

Princípios adotados:

- começar com a solução mais simples possível;
- evitar abstrações sem necessidade concreta;
- compreender a tecnologia antes de adicionar ferramentas que escondam sua complexidade;
- separar responsabilidades conforme o sistema cresce;
- utilizar necessidades reais do negócio para orientar a modelagem;
- preservar integridade e histórico dos dados;
- utilizar o PostgreSQL também como mecanismo de garantia de integridade;
- escrever testes automatizados para regras e integrações relevantes;
- evoluir o sistema através de pequenos incrementos;
- evitar padrões apenas porque são populares no mercado.

---

# Stack atual

## Aplicação

- .NET 10
- C#
- ASP.NET Core
- Minimal APIs
- Npgsql
- PostgreSQL

## Infraestrutura

- Docker
- Docker Compose

## Testes

- xUnit
- Microsoft.AspNetCore.Mvc.Testing
- `WebApplicationFactory`
- PostgreSQL real para testes de integração

## Desenvolvimento

- Git
- GitHub
- VS Code
- Insomnia

---

# Estrutura atual

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

A dependência entre os projetos segue apenas uma direção:

```text
SaborSante.Api.Tests
        ↓
SaborSante.Api
```

O projeto de produção não depende do projeto de testes.

---

# Arquitetura atual

A aplicação mantém uma arquitetura propositalmente simples:

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

## Clientes

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

## Endereços

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

As interfaces não foram criadas antecipadamente.

Elas surgiram quando os services precisaram ser testados isoladamente e a dependência direta dos repositories concretos começou a dificultar os testes.

---

# Evolução técnica do projeto

## 1 — Fundamentos da API

O projeto começou com ASP.NET Core e dados armazenados em memória.

Foram estudados:

- `Program.cs`;
- top-level statements;
- `WebApplicationBuilder`;
- `WebApplication`;
- Kestrel;
- middleware;
- Minimal APIs;
- routing;
- parâmetros de rota;
- model binding;
- lambdas;
- records;
- LINQ;
- JSON;
- REST;
- códigos de status HTTP;
- CRUD.

Primeiros endpoints:

```text
GET    /health
GET    /clientes
GET    /clientes/{id}
POST   /clientes
PUT    /clientes/{id}
DELETE /clientes/{id}
```

---

## 2 — Persistência com PostgreSQL

O armazenamento em memória foi substituído por PostgreSQL executando em Docker.

Foram estudados:

- PostgreSQL;
- Docker;
- Docker Compose;
- volumes;
- connection strings;
- Npgsql;
- SQL manual;
- SQL parametrizado;
- SQL Injection;
- identidade gerada pelo banco;
- `INSERT ... RETURNING`;
- leitura manual de resultados;
- mapeamento SQL → C#;
- `ExecuteScalarAsync`;
- `ExecuteReaderAsync`;
- `ExecuteNonQueryAsync`;
- `async`;
- `await`;
- `using`;
- `await using`;
- connection pooling.

---

## 3 — Separação de responsabilidades

Com o crescimento do `Program.cs`, a aplicação foi separada gradualmente em:

```text
Program
  ↓
Endpoints
  ↓
Services
  ↓
Repositories
```

Foram estudados:

- extension methods;
- Dependency Injection;
- dependências por construtor;
- Singleton;
- Scoped;
- Transient;
- `NpgsqlDataSource`;
- Repository;
- Service;
- separação entre HTTP, regras da aplicação e persistência.

---

## 4 — Validação e normalização

As primeiras regras de aplicação foram introduzidas no `ClienteService`.

Modelo atual de cliente:

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

Regras atuais incluem:

- nome obrigatório;
- telefone obrigatório;
- remoção de espaços desnecessários;
- telefone armazenado somente com dígitos;
- tratamento de valores `null`.

Exemplo:

```text
Entrada
(47) 99999-9999

↓ normalização

47999999999
```

Fluxo:

```text
Entrada externa
      ↓
Normalização
      ↓
Validação
      ↓
Persistência
```

---

## 5 — Result Pattern

Erros esperados da aplicação passaram a ser representados explicitamente através de:

```text
Resultado<T>
```

Tipos atuais de erro:

```text
Validacao
NaoEncontrado
Conflito
```

Tradução para HTTP:

```text
Validacao
→ 400 Bad Request

NaoEncontrado
→ 404 Not Found

Conflito
→ 409 Conflict
```

Foram estudados:

- generics;
- records com comportamento;
- factory methods;
- enums;
- switch expressions;
- erros esperados;
- exceptions inesperadas;
- separação entre resultado da aplicação e protocolo HTTP.

---

## 6 — Integridade e concorrência

A unicidade de telefone deixou de depender apenas da aplicação.

A regra também passou a ser garantida pelo PostgreSQL.

Foram estudados:

- constraints;
- índices únicos;
- race conditions;
- `PostgresException`;
- SQLSTATE;
- `23505 - unique_violation`;
- tradução de erros de persistência.

---

## 7 — Soft delete de clientes

Clientes passaram a ser inativados em vez de fisicamente removidos.

No PostgreSQL:

```sql
ativo BOOLEAN NOT NULL DEFAULT TRUE
```

A exclusão lógica utiliza:

```sql
UPDATE clientes
SET ativo = FALSE
WHERE id = @id
  AND ativo = TRUE;
```

O campo `ativo` existe na persistência, mas não faz parte do record público `Cliente`.

Foram estudados:

- soft delete;
- preservação histórica;
- ciclo de vida;
- estado de negócio;
- diferença entre `DELETE` HTTP e exclusão física.

---

## 8 — Unicidade parcial de telefone

A regra atual é:

> O telefone deve ser único entre clientes ativos.

Implementação:

```sql
CREATE UNIQUE INDEX ux_clientes_telefone_ativo
ON clientes (telefone)
WHERE ativo = TRUE;
```

Assim:

```text
cliente inativo com telefone X
+
novo cliente ativo com telefone X
→ permitido
```

mas:

```text
dois clientes ativos com telefone X
→ conflito
```

---

## 9 — Reativação de clientes

Endpoint:

```text
PATCH /clientes/{id}/reativar
```

Estados tratados:

```text
Reativado
NaoEncontrado
JaAtivo
Conflito
```

Possíveis respostas:

```text
cliente reativado
→ 204 No Content

cliente inexistente
→ 404 Not Found

cliente já ativo
→ 409 Conflict

telefone utilizado por outro cliente ativo
→ 409 Conflict
```

---

## 10 — Versionamento do banco

A estrutura do PostgreSQL passou a ser representada por scripts SQL versionados no Git:

```text
database/
├── 001_create_clientes.sql
└── 002_create_clientes_enderecos.sql
```

Ainda não existe uma ferramenta automatizada de migrations.

A intenção é compreender primeiro o problema antes de introduzir uma ferramenta para resolvê-lo.

---

# Endereços de clientes

## Relacionamento

Foi introduzido o primeiro relacionamento real do domínio:

```text
Cliente
1 ───────── N
          Endereços
```

Exemplo:

```text
Cliente
├── Casa
├── Trabalho
└── Outro
```

Isso representa uma necessidade real da operação: o mesmo cliente pode receber pedidos em endereços diferentes.

No banco:

```text
clientes.id
     ↑
     │ Foreign Key
     │
clientes_enderecos.cliente_id
```

---

## Modelo de endereço

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

Foram estudados:

- relacionamento um-para-muitos;
- foreign keys;
- integridade referencial;
- recursos pai e filho;
- rotas aninhadas;
- `DBNull.Value`;
- `reader.IsDBNull`;
- ownership.

---

## Endpoints de endereço

```text
GET    /clientes/{clienteId}/enderecos
GET    /clientes/{clienteId}/enderecos/{enderecoId}

POST   /clientes/{clienteId}/enderecos

PUT    /clientes/{clienteId}/enderecos/{enderecoId}

DELETE /clientes/{clienteId}/enderecos/{enderecoId}

PATCH  /clientes/{clienteId}/enderecos/{enderecoId}/reativar
```

Regras:

- cliente precisa existir;
- endereço precisa pertencer ao cliente informado;
- endereços possuem soft delete;
- endereços inativos não aparecem nas consultas normais;
- endereços podem ser reativados;
- campos obrigatórios são validados;
- campos opcionais vazios são normalizados para `null`.

Exemplo de ownership:

```text
endereço 10 pertence ao cliente 2

GET /clientes/2/enderecos/10
→ 200 OK

GET /clientes/3/enderecos/10
→ 404 Not Found
```

---

# Testes automatizados

A estratégia atual possui três níveis principais.

```text
                 HTTP
                  │
           Testes da API
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

Cada nível responde a perguntas diferentes.

---

# Testes unitários

Os services são testados isoladamente utilizando fakes manuais.

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

Os fakes permitem:

- configurar resultados;
- simular recursos existentes e inexistentes;
- capturar argumentos;
- contar chamadas;
- verificar se uma dependência foi utilizada;
- executar testes sem PostgreSQL.

Foram estudados:

- xUnit;
- `[Fact]`;
- Arrange / Act / Assert;
- test doubles;
- fake manual;
- `Task.FromResult`;
- object initializer;
- verificação de estado;
- verificação de interação;
- argumentos enviados;
- quantidade de chamadas.

---

## Cobertura do ClienteService

### Criação

Testes para:

- nome obrigatório;
- telefone obrigatório;
- valores `null`;
- telefone sem dígitos;
- normalização;
- sucesso;
- conflito;
- repository não chamado em validações inválidas;
- argumentos enviados ao repository.

### Atualização

Testes para:

- sucesso;
- cliente inexistente;
- conflito;
- normalização;
- validação;
- chamadas ao repository.

### Exclusão

Testes para:

- sucesso;
- cliente não excluído;
- ID enviado;
- quantidade de chamadas.

### Reativação

Testes para:

- reativado;
- não encontrado;
- já ativo;
- conflito;
- ID enviado;
- quantidade de chamadas.

---

## Cobertura do ClienteEnderecoService

### Criação

Testes para:

- cliente inexistente;
- sucesso;
- normalização;
- campos opcionais;
- validação dos campos obrigatórios;
- repository não chamado em cenários inválidos.

### Atualização

Testes para:

- sucesso;
- endereço inexistente;
- cliente inexistente;
- normalização;
- campos opcionais;
- validação;
- argumentos enviados.

### Exclusão

Testes para:

- sucesso;
- endereço inexistente;
- cliente inexistente;
- IDs enviados;
- chamadas ao repository.

### Reativação

Estados atuais:

```text
Reativado
NaoEncontrado
JaAtivo
```

Testes para:

- sucesso;
- endereço inexistente;
- endereço já ativo;
- cliente inexistente;
- IDs enviados;
- quantidade de chamadas.

---

# Interfaces e inversão de dependência

As interfaces surgiram durante os testes.

Inicialmente:

```text
ClienteService
      ↓
ClienteRepository
```

Ao tentar isolar o service, a dependência concreta dificultou o teste.

A estrutura evoluiu para:

```text
ClienteService
      ↓
IClienteRepository
      ↑
ClienteRepository
```

e:

```text
ClienteEnderecoService
        ↓
IClienteEnderecoRepository
        ↑
ClienteEnderecoRepository
```

Isso introduziu na prática:

- abstrações;
- contratos;
- substituição de implementações;
- redução de acoplamento;
- Dependency Injection através de interfaces;
- inversão de dependência.

A abstração não foi criada antecipadamente: surgiu a partir de um problema real de testabilidade.

---

# Testes de integração com PostgreSQL

Os testes unitários conseguem provar regras dos services, mas não conseguem provar que isto funciona:

```text
Repository
↓
Npgsql
↓
SQL
↓
PostgreSQL
```

Por exemplo, um fake não descobriria necessariamente:

- nome de tabela errado;
- coluna errada;
- SQL inválido;
- erro no `RETURNING`;
- tipo PostgreSQL incompatível;
- foreign key incorreta;
- índice inexistente.

Foi criado um banco separado:

```text
saborsante_tests
```

O banco de desenvolvimento continua sendo:

```text
saborsante
```

Assim:

```text
Desenvolvimento
→ saborsante

Testes
→ saborsante_tests
```

---

## PostgresFixture

A infraestrutura PostgreSQL utilizada pelos testes foi centralizada em:

```text
PostgresFixture
```

Ela é responsável por:

- carregar a configuração dos testes;
- criar o `NpgsqlDataSource`;
- compartilhar o datasource;
- descartar o recurso no final;
- disponibilizar repositories;
- limpar o banco entre testes.

Foram estudados:

- `IClassFixture`;
- `IAsyncLifetime`;
- ciclo de vida de recursos de teste;
- compartilhamento de infraestrutura;
- preparação e limpeza de dados.

---

## Limpeza e isolamento

Inicialmente cada teste possuía um `finally` que apagava manualmente os dados criados.

Isso gerou muita repetição.

A estratégia evoluiu para uma limpeza centralizada:

```text
LimparBancoAsync()
↓
DELETE clientes_enderecos
↓
DELETE clientes
```

Cada teste começa com um estado conhecido.

---

## Concorrência entre testes

Ao executar classes diferentes de integração simultaneamente, apareceu uma race condition.

Exemplo:

```text
Teste A
↓
cria cliente

              Teste B
              ↓
              limpa banco

Teste A continua
↓
cliente desapareceu
```

Também ocorreram conflitos com foreign keys durante limpezas concorrentes.

O problema apareceu porque várias classes utilizavam o mesmo banco mutável:

```text
saborsante_tests
```

A solução foi agrupar esses testes em uma collection do xUnit:

```text
Postgres Integration
```

com paralelização desabilitada.

Assim:

```text
Teste A
↓
termina

Teste B
↓
termina

Teste C
↓
termina
```

em vez de:

```text
Teste A ────────┐
                ├── mesmo banco
Teste B ────────┘
```

Conceitos estudados:

- isolamento;
- estado compartilhado;
- concorrência;
- race conditions;
- foreign key violations;
- xUnit collections;
- `DisableParallelization`.

---

# Testes de integração dos repositories

## ClienteRepository

São testados contra PostgreSQL real:

- `CriarAsync`;
- criação com telefone duplicado;
- `ObterPorIdAsync`;
- cliente inexistente;
- `AtualizarAsync`;
- cliente inexistente;
- conflito de telefone;
- `ExcluirAsync`;
- soft delete;
- exclusão inexistente;
- `ReativarAsync`;
- não encontrado;
- já ativo;
- conflito de telefone.

Esses testes exercitam realmente:

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

São testados:

- criação;
- leitura por ID;
- listagem;
- atualização;
- exclusão;
- reativação;
- soft delete;
- registros inativos;
- endereço inexistente;
- ownership;
- relacionamento com cliente;
- foreign key.

Exemplo:

```text
cliente 1 possui endereço 10

cliente 2 tenta atualizar endereço 10
↓
false
```

Assim a regra de ownership também é verificada diretamente na persistência.

---

# Testes automatizados da API HTTP

Depois dos testes unitários e dos testes dos repositories ainda existia uma lacuna:

```text
HTTP
↓
?
↓
Service
↓
Repository
```

Era necessário testar a aplicação a partir da sua fronteira externa.

Foi introduzido:

```text
Microsoft.AspNetCore.Mvc.Testing
```

e:

```text
WebApplicationFactory<Program>
```

Agora os testes conseguem executar:

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
Resposta HTTP
```

sem precisar iniciar manualmente a aplicação ou utilizar o Insomnia.

---

# HealthEndpointTests

O primeiro teste HTTP automatizado foi:

```text
GET /health
```

Ele verifica:

- aplicação inicializa;
- rota existe;
- resposta é exatamente `200 OK`;
- resposta é JSON;
- propriedade `status`;
- valor `"ok"`.

Foram utilizados:

- `HttpClient`;
- `HttpStatusCode`;
- `GetAsync`;
- `ReadFromJsonAsync<T>`.

---

# ApiWebApplicationFactory

Foi criada:

```text
ApiWebApplicationFactory
```

derivando de:

```csharp
WebApplicationFactory<Program>
```

Ela representa a aplicação utilizada pelos testes HTTP.

Durante sua implementação surgiu um problema importante.

Apenas adicionar:

```text
appsettings.IntegrationTests.json
```

à configuração não foi suficiente para fazer a aplicação utilizar `saborsante_tests`.

A aplicação ainda possuía o `NpgsqlDataSource` configurado pelo `Program.cs`.

O problema ficou evidente quando:

```text
PostgresFixture
→ limpava saborsante_tests

API
→ utilizava saborsante
```

Um teste que deveria retornar:

```text
201 Created
```

retornou:

```text
409 Conflict
```

porque o telefone já existia no banco de desenvolvimento.

A correção foi substituir explicitamente a dependência no container de testes:

```text
RemoveAll<NpgsqlDataSource>
↓
registrar NpgsqlDataSource de saborsante_tests
```

A estrutura passou a ser:

```text
PostgresFixture
        ↓
saborsante_tests
        ↑
ApiWebApplicationFactory
```

Foram estudados:

- `WebApplicationFactory`;
- host de testes ASP.NET Core;
- configuração de aplicação em testes;
- `ConfigureTestServices`;
- substituição de dependências;
- `RemoveAll<T>`;
- diferença entre configuração e instância já registrada no DI.

---

# Testes HTTP de clientes

## POST /clientes

São verificados:

```text
201 Created
400 Bad Request
409 Conflict
```

Além disso:

- JSON retornado;
- ID gerado;
- nome;
- normalização do telefone;
- conflito de unicidade.

---

## GET /clientes/{id}

São verificados:

```text
cliente existente
→ 200 OK

cliente inexistente
→ 404 Not Found
```

A resposta JSON também é desserializada e comparada.

---

## PUT /clientes/{id}

São verificados:

- atualização válida;
- `204 No Content`;
- persistência confirmada através de `GET`;
- cliente inexistente;
- `404 Not Found`;
- conflito de telefone;
- `409 Conflict`.

---

## DELETE /clientes/{id}

São verificados:

```text
cliente existente
→ 204 No Content

GET posterior
→ 404 Not Found
```

confirmando o soft delete.

Também é testado:

```text
cliente inexistente
→ 404 Not Found
```

---

## PATCH /clientes/{id}/reativar

São testados:

```text
cliente inativo
→ 204 No Content

cliente inexistente
→ 404 Not Found

cliente já ativo
→ 409 Conflict

telefone utilizado por outro cliente ativo
→ 409 Conflict
```

O cenário de conflito atravessa toda a pilha:

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

# Testes HTTP de endereços

## POST /clientes/{clienteId}/enderecos

São testados:

- criação válida;
- `201 Created`;
- JSON retornado;
- `clienteId` correto;
- campos do endereço;
- cliente inexistente;
- `404 Not Found`;
- dados inválidos;
- `400 Bad Request`.

---

## GET /clientes/{clienteId}/enderecos/{enderecoId}

São testados:

```text
endereço existente
→ 200 OK

endereço inexistente
→ 404 Not Found

endereço pertence a outro cliente
→ 404 Not Found
```

---

## GET /clientes/{clienteId}/enderecos

A listagem verifica que apenas endereços pertencentes ao cliente informado são retornados.

Exemplo:

```text
cliente 1
├── endereço A
└── endereço B

cliente 2
└── endereço C

GET /clientes/1/enderecos
→ A + B
```

---

## PUT /clientes/{clienteId}/enderecos/{enderecoId}

São testados:

- atualização válida;
- `204 No Content`;
- persistência confirmada por `GET`;
- endereço inexistente;
- ownership incorreto;
- `404 Not Found`.

---

## DELETE /clientes/{clienteId}/enderecos/{enderecoId}

São testados:

- soft delete;
- `204 No Content`;
- endereço deixa de ser encontrado;
- endereço inexistente;
- endereço de outro cliente;
- `404 Not Found`.

---

## PATCH /clientes/{clienteId}/enderecos/{enderecoId}/reativar

São testados:

```text
endereço inativo
→ 204 No Content

endereço já ativo
→ 409 Conflict

endereço inexistente
→ 404 Not Found

cliente inexistente
→ 404 Not Found
```

---

# Pirâmide atual de testes

Neste momento o projeto possui três níveis complementares.

## Unitário

```text
Service
↓
Fake
```

Objetivo:

> verificar regras isoladamente.

Vantagens:

- rápidos;
- determinísticos;
- sem infraestrutura externa.

---

## Integração de persistência

```text
Repository
↓
Npgsql
↓
PostgreSQL
```

Objetivo:

> verificar SQL, constraints, relacionamentos e persistência real.

---

## Integração HTTP

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

Objetivo:

> verificar a aplicação integrada a partir da fronteira HTTP.

Os níveis não se substituem.

Um teste HTTP não precisa reproduzir todos os casos já cobertos pelos testes unitários, e um teste unitário não consegue garantir que SQL e PostgreSQL estejam corretos.

---

# Conceitos de testes estudados até agora

- xUnit;
- `[Fact]`;
- Arrange / Act / Assert;
- unit tests;
- integration tests;
- testes HTTP;
- test doubles;
- fakes;
- estado configurável;
- interação;
- captura de argumentos;
- quantidade de chamadas;
- `Task.FromResult`;
- fixtures;
- `IClassFixture`;
- `IAsyncLifetime`;
- collections do xUnit;
- paralelização;
- race conditions;
- isolamento de testes;
- estado compartilhado;
- banco exclusivo para testes;
- preparação e limpeza de dados;
- `WebApplicationFactory`;
- `HttpClient`;
- `PostAsJsonAsync`;
- `PutAsJsonAsync`;
- `GetAsync`;
- `DeleteAsync`;
- `HttpRequestMessage`;
- `HttpMethod.Patch`;
- `ReadFromJsonAsync`;
- status HTTP;
- contrato JSON;
- substituição de dependências no DI.

Nenhuma biblioteca externa de mocks foi necessária até o momento.

---

# Modelo de domínio atual

```text
Cliente
   │
   └── Endereços
```

Ainda é um domínio pequeno, mas já possui:

- identidade;
- estado ativo/inativo;
- ciclo de vida;
- regras de unicidade;
- relacionamento;
- ownership;
- histórico preservado por soft delete;
- integridade referencial.

---

# Regras atuais

## Cliente

- nome obrigatório;
- telefone obrigatório;
- telefone normalizado;
- telefone único entre clientes ativos;
- cliente pode ser inativado;
- cliente pode ser reativado;
- reativação pode gerar conflito de telefone.

## Endereço

- pertence a exatamente um cliente;
- cliente precisa existir;
- identificação obrigatória;
- logradouro obrigatório;
- número obrigatório;
- bairro obrigatório;
- cidade obrigatória;
- complemento opcional;
- CEP opcional;
- campos opcionais vazios são convertidos para `null`;
- endereço possui soft delete;
- endereço pode ser reativado;
- endereço de um cliente não pode ser manipulado através de outro cliente.

---

# O que ainda não foi introduzido

O projeto ainda não utiliza formalmente:

- Clean Architecture;
- DDD;
- CQRS;
- MediatR;
- microservices;
- Event Sourcing;
- mensageria;
- Entity Framework Core;
- biblioteca de mocks;
- Testcontainers;
- migrations automatizadas;
- autenticação;
- autorização.

Isso é intencional.

Esses conceitos serão avaliados quando existirem problemas concretos que possam justificar sua utilização.

---

# Próxima etapa — Pedidos

A infraestrutura básica da API já possui:

```text
HTTP
✓

Services
✓

Repositories
✓

PostgreSQL
✓

Testes unitários
✓

Testes de integração
✓

Testes HTTP
✓
```

O próximo foco volta ao domínio da Sabor Santè.

A próxima área será:

```text
Pedido
```

O pedido começa a conectar entidades que hoje ainda estão relativamente independentes:

```text
Cliente
   ↓
Endereço
   ↓
Pedido
   ↓
Itens
```

Essa etapa deverá levantar questões importantes de domínio.

---

## Endereço histórico do pedido

Exemplo:

```text
Hoje:
Pedido #100
→ Rua A, 123

Amanhã:
cliente altera cadastro
→ Rua B, 500
```

O pedido antigo deveria continuar representando:

```text
Rua A, 123
```

porque aquele foi o endereço utilizado quando o pedido foi realizado.

Isso exigirá decidir como preservar uma fotografia histórica do endereço.

---

## Estado do pedido

Será necessário estudar possíveis estados, como:

```text
Recebido
↓
Confirmado
↓
Em produção
↓
Embalado
↓
Saiu para entrega
↓
Entregue
```

Também surgirão perguntas como:

- pedido pode ser alterado depois de confirmado?
- pode ser cancelado?
- quando deixa de poder ser alterado?
- como registrar transições de estado?

---

## Itens de pedido

O pedido também deverá representar:

- refeições;
- quantidades;
- tipos de refeição;
- adicionais;
- proteínas;
- frutas;
- saladas;
- personalizações;
- substituições;
- planos.

---

# Evolução futura do domínio

A visão de longo prazo é:

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

## Produção

Pedidos individuais deverão gerar necessidades agregadas para a cozinha.

Exemplo:

```text
João
→ 5 refeições com frango

Maria
→ 3 refeições com frango

Pedro
→ 4 refeições com frango
```

Produção:

```text
Frango
→ 12 porções
```

Essa etapa deverá introduzir problemas envolvendo:

- agregação;
- consultas;
- datas;
- quantidades;
- estados;
- consistência;
- transações;
- relatórios.

---

## Embalagem e separação

Depois da produção será necessário representar:

```text
produção concluída
↓
embalagem
↓
identificação
↓
separação por cliente
↓
separação por entrega
```

---

## Entrega

A operação de entrega deverá considerar:

- endereço escolhido;
- data;
- horário;
- entregador;
- status;
- agrupamento de entregas.

---

## Roteirização

Uma etapa posterior poderá estudar otimização das rotas de entrega.

Esse problema só será abordado quando existirem dados e fluxos suficientes para justificá-lo.

---

# Próximas evoluções técnicas possíveis

Além da evolução do domínio, problemas futuros poderão justificar o estudo de:

- migrations;
- Entity Framework Core;
- transações;
- autenticação;
- autorização;
- logging estruturado;
- tratamento global de erros;
- observabilidade;
- caching;
- background jobs;
- mensageria;
- concorrência;
- performance;
- CI/CD;
- deploy;
- containers para testes;
- Testcontainers;
- arquitetura de software mais avançada.

A ordem não é fixa.

O problema real encontrado durante o desenvolvimento continuará determinando qual conceito será estudado em seguida.

---

# Visão do projeto

A Sabor Santè envolve muito mais do que CRUD.

A operação possui desafios relacionados a:

- clientes;
- múltiplos endereços;
- cardápios semanais;
- refeições tradicionais;
- refeições low carb;
- refeições vegetarianas;
- saladas;
- proteínas;
- frutas;
- adicionais;
- planos;
- personalizações;
- substituições;
- pedidos;
- produção;
- capacidade diária;
- horários de corte;
- embalagem;
- separação;
- entregadores;
- entregas;
- roteirização.

A intenção é modelar essas áreas progressivamente, conforme cada problema passa a existir concretamente no sistema.

---

# Estado atual

Neste ponto o projeto já deixou de ser apenas um CRUD básico.

Ele possui:

```text
API REST
+
PostgreSQL
+
SQL manual
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
Validação
+
Normalização
+
Soft Delete
+
Reativação
+
Partial Unique Index
+
Relacionamentos
+
Foreign Keys
+
Ownership
+
Testes Unitários
+
Fakes
+
Testes de Integração
+
Banco exclusivo de testes
+
Fixtures
+
Isolamento
+
Controle de concorrência
+
Testes HTTP automatizados
+
WebApplicationFactory
```

O próximo objetivo é usar essa base para começar a modelar partes mais importantes do domínio da operação, começando por **Pedidos**.
