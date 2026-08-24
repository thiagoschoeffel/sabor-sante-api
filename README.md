# Sabor Santè API

API REST desenvolvida para apoiar a operação da **Sabor Santè**, uma empresa de alimentação saudável.

O projeto também funciona como um estudo progressivo de desenvolvimento backend com **.NET 10**, **ASP.NET Core** e **PostgreSQL**, partindo de uma implementação simples e evoluindo gradualmente conforme novos conceitos, problemas e necessidades reais surgem.

## Objetivo do projeto

Construir do zero uma API para a operação da Sabor Santè, começando com uma implementação simples e evoluindo gradualmente conforme novos conceitos são estudados e aplicados.

A intenção não é iniciar com uma arquitetura complexa pronta, mas entender **quais problemas levam à introdução de cada padrão, abstração e decisão arquitetural**.

A evolução do projeto busca conciliar dois objetivos:

- aprofundar conhecimentos em desenvolvimento backend, arquitetura de software e sistemas distribuídos;
- construir gradualmente uma solução que possa ser utilizada na operação real da Sabor Santè e, futuramente, adaptada para outras empresas do mesmo nicho.

## Princípios do estudo

O desenvolvimento segue algumas premissas:

- começar com a solução mais simples possível;
- evitar abstrações antes de existir um problema que as justifique;
- entender o funcionamento das tecnologias antes de utilizar ferramentas que escondam sua complexidade;
- introduzir arquitetura progressivamente;
- utilizar necessidades reais do negócio para orientar a modelagem;
- preservar integridade e histórico dos dados;
- evoluir o sistema em pequenos incrementos versionados.

## Stack atual

- .NET 10
- C#
- ASP.NET Core Minimal APIs
- PostgreSQL
- Npgsql
- Docker
- Docker Compose
- Git
- GitHub
- VS Code
- Insomnia

## Evolução do projeto

### Etapa 1 — Fundamentos da API

Primeiro contato com ASP.NET Core e construção da aplicação sem persistência externa.

Conceitos estudados:

- Minimal APIs;
- `Program.cs`;
- Kestrel;
- endpoints HTTP;
- routing;
- parâmetros de rota;
- model binding;
- serialização e desserialização JSON;
- códigos de status HTTP;
- `GET`;
- `POST`;
- `PUT`;
- `DELETE`;
- LINQ;
- records;
- coleções em memória;
- CRUD de clientes em memória.

Endpoints iniciais:

```text
GET    /health
GET    /clientes
GET    /clientes/{id}
POST   /clientes
PUT    /clientes/{id}
DELETE /clientes/{id}
```

---

### Etapa 2 — Persistência com PostgreSQL

Substituição do armazenamento em memória por persistência real.

Conceitos estudados:

- PostgreSQL em Docker;
- Docker Compose;
- volumes persistentes;
- conexão entre aplicação e banco;
- connection strings;
- Npgsql;
- SQL manual;
- SQL parametrizado;
- proteção contra SQL Injection;
- `ExecuteScalarAsync`;
- `ExecuteReaderAsync`;
- `ExecuteNonQueryAsync`;
- `async` / `await`;
- operações de I/O assíncronas;
- `using` e `await using`;
- descarte de recursos;
- connection pooling;
- mapeamento manual entre dados relacionais e objetos C#;
- identidade gerada pelo PostgreSQL;
- `INSERT ... RETURNING`.

Nesta etapa, todo o CRUD de clientes passou a utilizar o PostgreSQL como fonte de verdade.

---

### Etapa 3 — Separação de responsabilidades

Com o crescimento do `Program.cs` e dos endpoints, começaram a surgir responsabilidades diferentes dentro do mesmo código.

A aplicação foi progressivamente separada em:

```text
Program.cs
    ↓
ClienteEndpoints
    ↓
ClienteService
    ↓
ClienteRepository
    ↓
PostgreSQL
```

Conceitos estudados:

- extension methods;
- organização de endpoints;
- Dependency Injection;
- container de DI do ASP.NET Core;
- lifetimes:
  - Singleton;
  - Scoped;
  - Transient;

- `NpgsqlDataSource`;
- repository;
- service;
- separação entre HTTP, regras da aplicação e persistência;
- dependências por construtor;
- redução de acoplamento;
- diferença entre tipos externos e dados internos validados.

---

### Etapa 4 — Validação e normalização

As primeiras regras da aplicação foram introduzidas no cadastro e atualização de clientes.

Regras atuais:

- nome obrigatório;
- telefone obrigatório;
- remoção de espaços desnecessários no nome;
- telefone armazenado apenas com dígitos;
- tratamento seguro de valores `null`.

Exemplo:

```text
Entrada:
(47) 99999-9999

Armazenamento:
47999999999
```

Conceitos estudados:

- validação de entrada;
- normalização de dados;
- representação canônica;
- nullable reference types;
- `string` vs `string?`;
- null safety;
- operador `?.`;
- diferença entre erro do cliente e erro interno da aplicação;
- uso do sistema de tipos para representar garantias.

O fluxo passou a seguir:

```text
Entrada externa
    ↓
normalização
    ↓
validação
    ↓
dados internos válidos
    ↓
persistência
```

---

### Etapa 5 — Result Pattern e tratamento de erros

A aplicação deixou de utilizar exceções para representar erros esperados de validação.

Foi criado um `Resultado<T>` para representar explicitamente sucesso ou falha de uma operação.

Exemplo conceitual:

```text
Resultado<Cliente>

Sucesso
├── Valor = Cliente
└── Erro = null

Falha
├── Valor = null
└── Erro = mensagem
```

Também foram introduzidos tipos de erro:

```text
Validacao
Conflito
```

Permitindo que a camada HTTP traduza erros da aplicação para respostas adequadas:

```text
Validacao
→ 400 Bad Request

Conflito
→ 409 Conflict
```

Conceitos estudados:

- generics;
- `Resultado<T>`;
- factory methods;
- `Ok`;
- `Falha`;
- enums;
- switch expressions;
- diferença entre exceções inesperadas e erros esperados;
- separação entre erro da aplicação e protocolo HTTP;
- mapeamento de erros para status codes.

---

### Etapa 6 — Integridade e concorrência

Foi introduzida a regra de unicidade de telefone.

Inicialmente poderia parecer suficiente consultar antes de inserir:

```text
verifica telefone
    ↓
não existe
    ↓
insere
```

Porém, duas requisições simultâneas poderiam passar pela mesma verificação.

Por isso a integridade passou a ser garantida também pelo PostgreSQL.

Conceitos estudados:

- concorrência;
- race conditions;
- constraints;
- `UNIQUE`;
- `PostgresException`;
- SQLSTATE;
- `23505 - unique_violation`;
- tratamento específico de violações de integridade;
- `409 Conflict`;
- diferença entre regra da aplicação e garantia do banco.

---

### Etapa 7 — Ciclo de vida do cliente e Soft Delete

A exclusão física de clientes foi substituída por inativação.

Antes:

```sql
DELETE FROM clientes
WHERE id = @id;
```

Agora:

```sql
UPDATE clientes
SET ativo = FALSE
WHERE id = @id;
```

O objetivo é preservar histórico para futuras relações como:

- pedidos;
- endereços;
- planos;
- pagamentos;
- entregas.

Clientes inativos permanecem no banco, mas não aparecem nas consultas normais da API.

Conceitos estudados:

- soft delete;
- preservação histórica;
- estado de negócio;
- diferença entre remoção HTTP e exclusão física no banco;
- filtragem de registros ativos;
- integridade referencial futura.

---

### Etapa 8 — Unicidade parcial de telefone

Com o soft delete surgiu uma nova necessidade:

um cliente inativo não deveria reservar permanentemente um número de telefone.

A regra adotada passou a ser:

> O telefone deve ser único entre clientes ativos.

O PostgreSQL utiliza um índice único parcial:

```sql
CREATE UNIQUE INDEX ux_clientes_telefone_ativo
ON clientes (telefone)
WHERE ativo = TRUE;
```

Assim:

```text
cliente ativo + telefone X
→ outro ativo com telefone X não é permitido

cliente inativo + telefone X
→ novo cliente ativo com telefone X é permitido
```

Conceitos estudados:

- partial indexes;
- partial unique indexes;
- identidade técnica vs chave de negócio;
- unicidade condicional;
- regras de integridade relacionadas ao estado do registro.

---

### Etapa 9 — Reativação de clientes

Clientes inativos podem ser reativados por meio de uma operação explícita:

```text
PATCH /clientes/{id}/reativar
```

Cenários tratados:

```text
cliente inativo
→ 204 No Content

cliente já ativo
→ 409 Conflict

cliente inexistente
→ 404 Not Found

telefone utilizado por outro cliente ativo
→ 409 Conflict
```

A reativação representa uma transição de estado do domínio:

```text
Ativo
  ↓
Inativo
  ↓
Ativo novamente
```

Conceitos estudados:

- `PATCH`;
- transições de estado;
- endpoints orientados à intenção;
- diferença entre alterar um campo e executar uma operação de negócio;
- tratamento de conflitos durante reativação.

---

### Etapa 10 — Versionamento do banco de dados

A estrutura inicial do PostgreSQL passou a ser documentada e versionada no repositório.

Estrutura atual:

```text
database/
└── 001_create_clientes.sql
```

O objetivo é permitir que o schema faça parte do histórico do projeto juntamente com o código-fonte.

Neste momento o projeto utiliza scripts SQL simples.

Ferramentas de migrations serão introduzidas posteriormente, quando houver necessidade suficiente para justificar essa evolução.

Conceitos estudados:

- schema versionado;
- reprodutibilidade;
- infraestrutura como parte do código;
- evolução incremental do banco de dados.

## Modelo atual de cliente

Atualmente um cliente possui:

```text
Cliente
├── Id
├── Nome
├── Telefone
└── Ativo
```

Regras atuais:

- nome obrigatório;
- telefone obrigatório;
- telefone normalizado para apenas dígitos;
- telefone único entre clientes ativos;
- clientes não são removidos fisicamente;
- clientes inativos podem ser reativados;
- conflito de telefone é protegido também pelo PostgreSQL.

## Próximas evoluções

As próximas etapas serão introduzidas conforme surgirem necessidades reais no domínio.

Entre os assuntos planejados estão:

- endereços de clientes;
- relacionamento um-para-muitos;
- pedidos;
- itens de pedido;
- cardápios;
- planos de refeições;
- produção agregada;
- embalagem;
- entregadores;
- rotas de entrega;
- testes automatizados;
- autenticação e autorização;
- migrations;
- Entity Framework Core;
- logging estruturado;
- tratamento global de erros;
- observabilidade;
- caching;
- jobs;
- mensageria;
- concorrência;
- transações;
- performance;
- arquitetura de software;
- escalabilidade.

Esses recursos não serão introduzidos apenas por serem populares. Cada conceito será aplicado quando o crescimento da aplicação apresentar um problema concreto que justifique sua utilização.

## Visão de longo prazo

O objetivo é evoluir a aplicação até que ela seja capaz de apoiar todo o fluxo operacional da Sabor Santè:

```text
Cliente
   ↓
Cardápio / Plano
   ↓
Pedido
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

A intenção é transformar gradualmente o projeto de estudo em uma solução utilizável na operação real da Sabor Santè e, futuramente, avaliar sua aplicação em outras empresas do segmento de alimentação saudável.
