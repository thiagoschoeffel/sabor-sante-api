# Sabor Santè API

API REST desenvolvida para apoiar a operação da **Sabor Santè**, uma empresa de alimentação saudável.

O projeto também funciona como um estudo progressivo de desenvolvimento backend com **.NET 10**, **ASP.NET Core** e **PostgreSQL**, partindo de implementações simples e evoluindo conforme problemas reais justificam novos conceitos, abstrações e decisões arquiteturais.

## Objetivo do projeto

Construir do zero uma API capaz de apoiar gradualmente a operação da Sabor Santè.

O projeto possui dois objetivos complementares:

- aprofundar conhecimentos em desenvolvimento backend, arquitetura de software, bancos de dados e sistemas distribuídos;
- evoluir para uma solução que possa ser utilizada na operação real da Sabor Santè e, futuramente, adaptada para outras empresas do segmento de alimentação saudável.

A intenção não é começar com uma arquitetura complexa pronta.

Cada padrão, abstração ou ferramenta deve surgir a partir de uma necessidade concreta encontrada durante a evolução do sistema.

## Princípios do estudo

- começar com a solução mais simples possível;
- evitar abstrações antes de existir um problema que as justifique;
- entender o funcionamento das tecnologias antes de adicionar ferramentas que escondam sua complexidade;
- introduzir arquitetura progressivamente;
- utilizar necessidades reais do negócio para orientar a modelagem;
- separar responsabilidades conforme o sistema cresce;
- preservar integridade e histórico dos dados;
- utilizar o banco de dados também como mecanismo de garantia de integridade;
- escrever testes quando regras suficientes surgirem para justificar automação;
- evoluir o sistema em pequenos incrementos versionados;
- evitar implementar padrões apenas porque são populares no mercado.

## Stack atual

- .NET 10
- C#
- ASP.NET Core
- Minimal APIs
- PostgreSQL
- Npgsql
- Docker
- Docker Compose
- xUnit
- Git
- GitHub
- VS Code
- Insomnia

## Estrutura atual da solution

```text
SaborSante/
├── README.md
├── SaborSante.slnx
│
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
│       │
│       ├── Resultado.cs
│       ├── Program.cs
│       ├── docker-compose.yml
│       └── SaborSante.Api.csproj
│
└── tests/
    └── SaborSante.Api.Tests/
        ├── ClienteServiceTests.cs
        ├── FakeClienteRepository.cs
        └── SaborSante.Api.Tests.csproj
```

A separação entre `src/` e `tests/` mantém o código de produção e o código responsável por verificá-lo organizados dentro da mesma solution.

A dependência entre os projetos segue apenas uma direção:

```text
SaborSante.Api.Tests
        ↓
SaborSante.Api
```

A aplicação de produção não possui dependência do projeto de testes.

---

# Evolução do projeto

## Etapa 1 — Fundamentos da API

O projeto começou utilizando apenas ASP.NET Core e armazenamento em memória.

Conceitos estudados:

- `Program.cs`;
- top-level statements;
- `WebApplicationBuilder`;
- `WebApplication`;
- Kestrel;
- middleware;
- ambiente de execução;
- Minimal APIs;
- routing;
- parâmetros de rota;
- model binding;
- lambdas;
- serialização e desserialização JSON;
- records;
- LINQ;
- códigos de status HTTP;
- REST;
- CRUD;
- `GET`;
- `POST`;
- `PUT`;
- `DELETE`.

Primeiros endpoints:

```text
GET    /health

GET    /clientes
GET    /clientes/{id}
POST   /clientes
PUT    /clientes/{id}
DELETE /clientes/{id}
```

Inicialmente, os clientes eram armazenados em uma coleção em memória.

---

## Etapa 2 — Persistência com PostgreSQL

O armazenamento em memória foi substituído por persistência real utilizando PostgreSQL.

O PostgreSQL passou a executar em Docker com volume persistente.

Conceitos estudados:

- PostgreSQL;
- Docker;
- Docker Compose;
- volumes;
- connection strings;
- Npgsql;
- SQL manual;
- SQL parametrizado;
- proteção contra SQL Injection;
- identidade gerada pelo banco;
- `INSERT ... RETURNING`;
- leitura manual de resultados;
- mapeamento entre linhas SQL e objetos C#.

Também foram estudados:

```text
ExecuteScalarAsync
ExecuteReaderAsync
ExecuteNonQueryAsync
```

Além de:

- `async`;
- `await`;
- operações de I/O;
- `using`;
- `await using`;
- descarte determinístico de recursos;
- connection pooling.

---

## Etapa 3 — Separação de responsabilidades

Com o crescimento dos endpoints, o `Program.cs` começou a acumular responsabilidades.

A aplicação foi progressivamente organizada em:

```text
Program.cs
    ↓
Endpoints
    ↓
Service
    ↓
Repository
    ↓
PostgreSQL
```

Para clientes:

```text
ClienteEndpoints
      ↓
ClienteService
      ↓
ClienteRepository
      ↓
NpgsqlDataSource
      ↓
PostgreSQL
```

Conceitos estudados:

- extension methods;
- classes estáticas;
- Dependency Injection;
- container de DI do ASP.NET Core;
- constructor injection;
- Singleton;
- Scoped;
- Transient;
- `NpgsqlDataSource`;
- Repository;
- Service;
- separação entre HTTP, regras da aplicação e persistência.

Nenhuma interface foi criada nessa etapa porque ainda não existia uma necessidade concreta de substituição do repository.

---

## Etapa 4 — Validação e normalização

As primeiras regras de aplicação passaram a existir no `ClienteService`.

Modelo atual:

```csharp
public record Cliente(
    int Id,
    string Nome,
    string Telefone
);

public record CriarClienteRequest(
    string? Nome,
    string? Telefone
);

public record AtualizarClienteRequest(
    string? Nome,
    string? Telefone
);
```

Os requests aceitam valores nullable porque representam dados vindos de uma fronteira externa: HTTP.

Regras atuais:

- nome obrigatório;
- telefone obrigatório;
- remoção de espaços desnecessários no nome;
- telefone armazenado apenas com dígitos;
- tratamento seguro de `null`.

Exemplo:

```text
Entrada:
(47) 99999-9999

Armazenamento:
47999999999
```

Conceitos estudados:

- validação;
- normalização;
- representação canônica;
- nullable reference types;
- `string`;
- `string?`;
- operador `?.`;
- `string.IsNullOrWhiteSpace`;
- diferença entre dados externos e dados já validados internamente.

O fluxo adotado passou a ser:

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

## Etapa 5 — Result Pattern

Erros esperados da aplicação deixaram de depender exclusivamente de exceptions.

Foi criado um tipo genérico:

```text
Resultado<T>
```

para representar explicitamente sucesso ou falha.

Tipos atuais de erro:

```text
Validacao
NaoEncontrado
Conflito
```

Mapeamento para HTTP:

```text
Validacao
→ 400 Bad Request

NaoEncontrado
→ 404 Not Found

Conflito
→ 409 Conflict
```

Conceitos estudados:

- generics;
- `T`;
- records com comportamento;
- factory methods;
- enums;
- switch expressions;
- erros esperados;
- exceptions inesperadas;
- separação entre resultado da aplicação e protocolo HTTP.

---

## Etapa 6 — Integridade e concorrência

O telefone do cliente passou a possuir uma regra de unicidade.

Inicialmente a regra foi tratada pela aplicação, mas isso não era suficiente diante de requisições concorrentes.

A garantia passou a existir também no PostgreSQL.

Conceitos estudados:

- constraints;
- unique constraints;
- concorrência;
- race conditions;
- integridade no banco;
- `PostgresException`;
- SQLSTATE;
- `23505`;
- `unique_violation`;
- tradução de erros do PostgreSQL para erros da aplicação;
- `409 Conflict`.

---

## Etapa 7 — Soft Delete de clientes

A exclusão física de clientes foi substituída por inativação.

A tabela passou a possuir:

```sql
ativo BOOLEAN NOT NULL DEFAULT TRUE
```

Antes:

```sql
DELETE FROM clientes
WHERE id = @id;
```

Agora:

```sql
UPDATE clientes
SET ativo = FALSE
WHERE id = @id
  AND ativo = TRUE;
```

Clientes inativos deixam de aparecer nas consultas normais, mas permanecem armazenados.

Conceitos estudados:

- soft delete;
- ciclo de vida;
- preservação histórica;
- estado de negócio;
- diferença entre `DELETE` HTTP e `DELETE` SQL.

---

## Etapa 8 — Unicidade parcial de telefone

A regra de negócio adotada passou a ser:

> O telefone deve ser único entre clientes ativos.

Um telefone não funciona como identidade técnica permanente do cliente.

A identidade técnica continua sendo:

```text
clientes.id
```

A regra é garantida por um índice único parcial:

```sql
CREATE UNIQUE INDEX ux_clientes_telefone_ativo
ON clientes (telefone)
WHERE ativo = TRUE;
```

Isso permite que um cliente inativo tenha o mesmo telefone de um novo cliente ativo, mas impede dois clientes ativos com o mesmo telefone.

Conceitos estudados:

- indexes;
- partial indexes;
- partial unique indexes;
- identidade técnica;
- chave de negócio;
- unicidade condicional.

---

## Etapa 9 — Reativação de clientes

Foi adicionada uma operação explícita para reativação:

```text
PATCH /clientes/{id}/reativar
```

Estados tratados pelo repository:

```text
Reativado
NaoEncontrado
JaAtivo
Conflito
```

Cenários:

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

Conceitos estudados:

- `PATCH`;
- transições de estado;
- endpoints orientados à intenção;
- conflito de estado;
- conflitos de integridade durante transições.

---

## Etapa 10 — Versionamento do banco

As alterações realizadas manualmente no PostgreSQL começaram a ser representadas no Git.

Estrutura atual:

```text
database/
├── 001_create_clientes.sql
└── 002_create_clientes_enderecos.sql
```

Neste momento o projeto utiliza scripts SQL simples.

Ferramentas automatizadas de migration ainda não foram introduzidas porque a intenção é compreender primeiro o problema que elas resolvem.

Conceitos estudados:

- versionamento de schema;
- reprodução da estrutura do banco;
- banco como parte versionada do sistema;
- diferença entre alteração manual e alteração rastreável.

---

## Etapa 11 — Endereços de clientes

Foi introduzido o primeiro relacionamento real entre entidades do domínio.

A regra é:

```text
Cliente
1 ───────── N
          Endereços
```

Um cliente pode possuir múltiplos endereços.

Exemplo:

```text
Cliente
├── Casa
├── Trabalho
└── Outro
```

Isso representa uma necessidade real do negócio: uma entrega pode ser realizada no trabalho em determinado dia e na residência em outro.

Tabela:

```text
clientes_enderecos
```

Relacionamento:

```text
clientes.id
     ↑
     │ Foreign Key
     │
clientes_enderecos.cliente_id
```

Conceitos estudados:

- relacionamento um-para-muitos;
- foreign keys;
- integridade referencial;
- recursos filhos;
- nullable no banco;
- `DBNull.Value`;
- `reader.IsDBNull`;
- rotas aninhadas.

---

## Etapa 12 — CRUD e ciclo de vida de endereços

Endpoints atuais:

```text
GET    /clientes/{clienteId}/enderecos

GET    /clientes/{clienteId}/enderecos/{enderecoId}

POST   /clientes/{clienteId}/enderecos

PUT    /clientes/{clienteId}/enderecos/{enderecoId}

DELETE /clientes/{clienteId}/enderecos/{enderecoId}

PATCH  /clientes/{clienteId}/enderecos/{enderecoId}/reativar
```

Regras implementadas:

- o cliente precisa existir;
- um endereço pertence a um cliente;
- um endereço de outro cliente não pode ser acessado através da rota incorreta;
- endereços inativos não aparecem nas consultas normais;
- exclusão utiliza soft delete;
- endereços podem ser reativados;
- campos obrigatórios são validados;
- campos opcionais podem ser armazenados como `NULL`;
- o `clienteId` da rota é a fonte de verdade para a relação.

Exemplo de ownership:

```text
/endereco 10 pertence ao cliente 2

GET /clientes/2/enderecos/10
→ 200

GET /clientes/3/enderecos/10
→ 404
```

Conceitos estudados:

- ownership;
- autorização estrutural de recurso;
- pertencimento;
- recurso pai;
- recurso filho;
- coleção vazia vs recurso inexistente;
- soft delete em recurso dependente;
- reativação em recurso dependente.

---

## Etapa 13 — Testes unitários

Com o crescimento das regras do `ClienteService`, os testes manuais realizados pelo Insomnia deixaram de ser suficientes para garantir que mudanças futuras não quebrassem comportamentos já implementados.

Foi criado um projeto separado:

```text
tests/
└── SaborSante.Api.Tests/
```

utilizando **xUnit**.

O primeiro teste foi propositalmente simples para verificar o funcionamento da infraestrutura de testes.

Depois disso, começaram os testes reais do `ClienteService`.

Conceitos estudados:

- testes automatizados;
- testes unitários;
- xUnit;
- `[Fact]`;
- `Assert`;
- Arrange;
- Act;
- Assert.

Estrutura conceitual:

```text
Arrange
↓
preparar cenário

Act
↓
executar comportamento

Assert
↓
verificar resultado
```

---

## Etapa 14 — Inversão de dependência motivada por testes

Ao tentar testar `ClienteService`, surgiu um problema concreto.

O service dependia diretamente de:

```text
ClienteRepository
```

que por sua vez dependia de:

```text
NpgsqlDataSource
↓
PostgreSQL
```

O primeiro teste precisou temporariamente fazer algo equivalente a:

```csharp
new ClienteService(null!)
```

Isso evidenciou o acoplamento.

Foi então introduzida:

```csharp
IClienteRepository
```

A estrutura passou de:

```text
ClienteService
      ↓
ClienteRepository
      ↓
PostgreSQL
```

para:

```text
ClienteService
      ↓
IClienteRepository
      ↑
ClienteRepository
      ↓
PostgreSQL
```

Essa foi a primeira interface do projeto criada por uma necessidade concreta, e não antecipadamente.

Conceitos estudados:

- interface;
- contrato;
- implementação concreta;
- substituição de dependências;
- inversão de dependência;
- redução de acoplamento;
- Dependency Injection através de interfaces.

---

## Etapa 15 — Fake manual

Para testar o service sem PostgreSQL, foi criada uma implementação manual de:

```text
IClienteRepository
```

chamada:

```text
FakeClienteRepository
```

Nos testes:

```text
ClienteService
      ↓
IClienteRepository
      ↑
FakeClienteRepository
```

O fake permite configurar resultados.

Exemplo conceitual:

```text
ClienteParaRetornar = cliente
```

ou:

```text
ClienteParaRetornar = null
```

Também permite observar interações:

```text
NomeRecebido
TelefoneRecebido
IdRecebidoAtualizar
QuantidadeChamadasCriarAsync
QuantidadeChamadasAtualizarAsync
QuantidadeChamadasReativarAsync
```

Conceitos estudados:

- test doubles;
- fake manual;
- estado configurável;
- comportamento configurável;
- `Task.FromResult`;
- object initializer;
- observação de chamadas;
- verificação de argumentos;
- verificação da quantidade de chamadas.

Ainda não foi introduzida nenhuma biblioteca de mocks.

A intenção foi entender primeiro o mecanismo manualmente.

---

## Etapa 16 — Cobertura atual do ClienteService

A suíte possui atualmente **17 testes reais**, todos relacionados a comportamentos da aplicação.

### Criação de clientes

São testados cenários como:

- nome vazio;
- telefone vazio;
- nome `null`;
- telefone `null`;
- telefone sem dígitos;
- criação válida;
- telefone em conflito;
- normalização de nome;
- normalização de telefone;
- repository não chamado quando a validação falha;
- repository chamado exatamente uma vez em cenário válido;
- argumentos normalizados enviados ao repository.

### Atualização de clientes

São testados:

- atualização bem-sucedida;
- cliente não encontrado;
- telefone em conflito;
- normalização;
- validação;
- repository não chamado quando os dados são inválidos;
- repository chamado exatamente uma vez quando os dados são válidos;
- id correto enviado ao repository.

### Reativação de clientes

São testados:

- reativação bem-sucedida;
- cliente não encontrado;
- cliente já ativo;
- conflito de telefone;
- id enviado ao repository;
- quantidade de chamadas ao repository.

Os testes verificam tanto:

```text
Estado
→ o resultado retornado está correto?
```

quanto:

```text
Interação
→ a dependência foi chamada corretamente?
```

---

# Arquitetura atual

A arquitetura atual continua propositalmente simples:

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

Para clientes, após a introdução da primeira abstração:

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

Nos testes:

```text
ClienteServiceTests
      ↓
ClienteService
      ↓
IClienteRepository
      ↑
FakeClienteRepository
```

O projeto ainda não utiliza formalmente:

- Clean Architecture;
- DDD;
- CQRS;
- MediatR;
- microservices;
- Event Sourcing;
- mensageria;
- Entity Framework Core.

Esses conceitos só serão introduzidos caso o crescimento do sistema apresente problemas que justifiquem sua adoção.

---

# Modelo atual

## Cliente

```text
Cliente
├── Id
├── Nome
└── Telefone
```

O estado ativo/inativo atualmente faz parte da persistência e das regras de consulta, mas não compõe o record `Cliente` retornado pela aplicação.

## Endereço

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

Relacionamento:

```text
Cliente
   │
   ├── Endereço Casa
   ├── Endereço Trabalho
   └── Endereço Outro
```

---

# Regras de negócio atuais

Entre as regras já implementadas estão:

- clientes possuem identidade técnica própria;
- telefone é normalizado antes da persistência;
- telefone precisa ser único entre clientes ativos;
- clientes podem ser inativados;
- clientes podem ser reativados;
- reativação pode gerar conflito de telefone;
- clientes podem possuir múltiplos endereços;
- endereços pertencem a um cliente específico;
- endereços também possuem ciclo ativo/inativo;
- endereços inativos podem ser reativados;
- recursos filhos não podem ser manipulados através de outro cliente.

---

# Fluxo de desenvolvimento atual

O desenvolvimento procura seguir este ciclo:

```text
Necessidade real
      ↓
Implementação simples
      ↓
Problema aparece
      ↓
Entendimento do problema
      ↓
Introdução de conceito/padrão
      ↓
Teste
      ↓
Versionamento
```

Um exemplo concreto já ocorrido:

```text
ClienteService depende de ClienteRepository
      ↓
difícil testar isoladamente
      ↓
primeiro teste utiliza null!
      ↓
problema fica evidente
      ↓
IClienteRepository é introduzido
      ↓
FakeClienteRepository é criado
      ↓
service pode ser testado sem PostgreSQL
```

Esse processo representa a filosofia central do projeto.

---

# Próximas evoluções

Entre os assuntos planejados estão:

- ampliar testes automatizados;
- testes de `ClienteEnderecoService`;
- testes de integração;
- pedidos;
- itens de pedido;
- cardápios;
- planos de refeições;
- refeições personalizadas;
- substituições;
- produção agregada;
- embalagem;
- separação;
- entregadores;
- entregas;
- roteirização;
- limites de produção;
- horários de corte;
- capacidade diária;
- autenticação;
- autorização;
- migrations;
- Entity Framework Core;
- logging estruturado;
- tratamento global de erros;
- observabilidade;
- transações;
- concorrência;
- caching;
- jobs;
- mensageria;
- performance;
- deploy;
- CI/CD;
- arquitetura de software;
- escalabilidade.

Esses recursos não serão implementados simplesmente porque são comuns no mercado.

Cada um deverá surgir a partir de uma necessidade concreta da aplicação.

---

# Visão de domínio

A Sabor Santè trabalha com uma operação que envolve muito mais do que um CRUD simples.

O fluxo de longo prazo deve representar algo próximo de:

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
Necessidade agregada da cozinha
   ↓
Embalagem
   ↓
Separação
   ↓
Entrega
   ↓
Roteirização
```

Entre as necessidades previstas estão:

- múltiplos endereços por cliente;
- escolha do endereço em cada entrega;
- cardápios semanais ou diários;
- refeições tradicionais;
- refeições low carb;
- refeições vegetarianas;
- saladas;
- adicionais;
- proteínas;
- frutas;
- planos com diferentes quantidades de refeições;
- refeições personalizadas;
- substituições;
- consolidação da produção da cozinha;
- separação e embalagem;
- controle de entregas;
- organização de entregadores;
- otimização de rotas.

A modelagem dessas áreas será realizada gradualmente conforme cada parte do domínio começar a ser implementada.

---

# Visão de longo prazo

A intenção é transformar gradualmente este projeto de estudo em uma solução capaz de apoiar a operação real da **Sabor Santè**.

Caso a solução amadureça o suficiente, também poderá ser avaliada como base para atender outras empresas do segmento de alimentação saudável e produção de refeições.

O objetivo técnico não é apenas chegar a uma aplicação funcional, mas compreender profundamente as decisões que levam um sistema simples a evoluir para uma aplicação backend profissional.
