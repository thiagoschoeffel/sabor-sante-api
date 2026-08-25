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
- ASP.NET Core Minimal APIs
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
├── src/
│   └── SaborSante.Api/
│       ├── database/
│       │   ├── 001_create_clientes.sql
│       │   └── 002_create_clientes_enderecos.sql
│       ├── Cliente.cs
│       ├── ClienteEndpoints.cs
│       ├── ClienteService.cs
│       ├── ClienteRepository.cs
│       ├── IClienteRepository.cs
│       ├── ClienteEndereco.cs
│       ├── ClienteEnderecoEndpoints.cs
│       ├── ClienteEnderecoService.cs
│       ├── ClienteEnderecoRepository.cs
│       ├── IClienteEnderecoRepository.cs
│       ├── Resultado.cs
│       ├── Program.cs
│       ├── docker-compose.yml
│       └── SaborSante.Api.csproj
└── tests/
    └── SaborSante.Api.Tests/
        ├── ClienteServiceTests.cs
        ├── ClienteEnderecoServiceTests.cs
        ├── FakeClienteRepository.cs
        ├── FakeClienteEnderecoRepository.cs
        └── SaborSante.Api.Tests.csproj
```

A separação entre `src/` e `tests/` mantém o código de produção e o código responsável por verificá-lo organizados dentro da mesma solution.

A dependência entre os projetos segue apenas uma direção:

```text
SaborSante.Api.Tests
        ↓
SaborSante.Api
```

A aplicação de produção não depende do projeto de testes.

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
- mapeamento entre linhas SQL e objetos C#;
- `ExecuteScalarAsync`;
- `ExecuteReaderAsync`;
- `ExecuteNonQueryAsync`;
- `async`;
- `await`;
- operações de I/O;
- `using`;
- `await using`;
- descarte determinístico de recursos;
- connection pooling.

---

## Etapa 3 — Separação de responsabilidades

Com o crescimento do `Program.cs` e dos endpoints, começaram a surgir responsabilidades diferentes dentro do mesmo código.

A aplicação foi progressivamente separada em:

```text
Program.cs
    ↓
Endpoints
    ↓
Services
    ↓
Repositories
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
- organização de endpoints;
- Dependency Injection;
- container de DI do ASP.NET Core;
- dependências por construtor;
- Singleton;
- Scoped;
- Transient;
- `NpgsqlDataSource`;
- repository;
- service;
- separação entre HTTP, aplicação e persistência;
- redução de acoplamento.

Nenhuma interface foi criada inicialmente porque ainda não existia uma necessidade concreta de substituir os repositories.

---

## Etapa 4 — Validação e normalização

As primeiras regras de aplicação foram introduzidas no `ClienteService`.

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

Os requests aceitam referências nullable porque representam dados recebidos através de uma fronteira externa: HTTP.

Regras atuais:

- nome obrigatório;
- telefone obrigatório;
- remoção de espaços desnecessários;
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

- validação;
- normalização;
- representação canônica;
- nullable reference types;
- `string` vs `string?`;
- null safety;
- operador `?.`;
- `string.IsNullOrWhiteSpace`.

O fluxo adotado é:

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

## Etapa 5 — Result Pattern e tratamento de erros

Erros esperados deixaram de ser representados exclusivamente por exceptions.

Foi criado:

```text
Resultado<T>
```

para representar explicitamente sucesso ou falha.

Tipos atuais:

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

Foi introduzida a regra de unicidade de telefone.

A garantia passou a existir também no PostgreSQL para evitar problemas de concorrência.

Conceitos estudados:

- concorrência;
- race conditions;
- constraints;
- `UNIQUE`;
- `PostgresException`;
- SQLSTATE;
- `23505 - unique_violation`;
- integridade garantida pelo banco;
- tradução de erros de persistência;
- `409 Conflict`.

---

## Etapa 7 — Soft Delete de clientes

A exclusão física de clientes foi substituída por inativação.

A tabela possui:

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

Clientes inativos deixam de aparecer nas consultas normais, mas permanecem armazenados.

O campo `ativo` pertence atualmente ao modelo de persistência e às regras de consulta. Ele não faz parte do record `Cliente`.

Conceitos estudados:

- soft delete;
- ciclo de vida;
- preservação histórica;
- estado de negócio;
- diferença entre `DELETE` HTTP e exclusão física no banco.

---

## Etapa 8 — Unicidade parcial de telefone

A regra atual é:

> O telefone deve ser único entre clientes ativos.

Isso é garantido por:

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

dois clientes ativos com telefone X
→ não permitido
```

Conceitos estudados:

- indexes;
- partial indexes;
- partial unique indexes;
- identidade técnica;
- chave de negócio;
- unicidade condicional.

---

## Etapa 9 — Reativação de clientes

Clientes inativos podem ser reativados através de:

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
- conflitos de estado;
- conflitos de integridade durante transições.

---

## Etapa 10 — Versionamento do banco

A estrutura do PostgreSQL passou a ser representada no Git através de scripts SQL.

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
- diferença entre alterações manuais e alterações rastreáveis.

---

## Etapa 11 — Endereços de clientes

Foi introduzido o primeiro relacionamento real do domínio:

```text
Cliente
1 ───────── N
          Endereços
```

Um cliente pode possuir múltiplos endereços:

```text
Cliente
├── Casa
├── Trabalho
└── Outro
```

Isso representa uma necessidade real da operação: uma entrega pode ser realizada no trabalho em determinado dia e na residência em outro.

Relacionamento no banco:

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
- recursos dependentes;
- recursos filhos;
- rotas aninhadas;
- `DBNull.Value`;
- `reader.IsDBNull`;
- ownership.

---

## Etapa 12 — CRUD e ciclo de vida de endereços

Endpoints:

```text
GET    /clientes/{clienteId}/enderecos
GET    /clientes/{clienteId}/enderecos/{enderecoId}
POST   /clientes/{clienteId}/enderecos
PUT    /clientes/{clienteId}/enderecos/{enderecoId}
DELETE /clientes/{clienteId}/enderecos/{enderecoId}
PATCH  /clientes/{clienteId}/enderecos/{enderecoId}/reativar
```

Regras:

- o cliente precisa existir;
- o endereço precisa pertencer ao cliente informado;
- endereços inativos não aparecem nas consultas normais;
- exclusão utiliza soft delete;
- endereços podem ser reativados;
- campos obrigatórios são validados;
- campos opcionais podem ser armazenados como `NULL`;
- `clienteId` da rota é a fonte de verdade para o relacionamento.

Exemplo de ownership:

```text
endereço 10 pertence ao cliente 2

GET /clientes/2/enderecos/10
→ 200 OK

GET /clientes/3/enderecos/10
→ 404 Not Found
```

Conceitos estudados:

- ownership;
- pertencimento;
- recurso pai;
- recurso filho;
- coleção vazia vs recurso inexistente;
- soft delete em recursos dependentes;
- reativação;
- proteção contra acesso cruzado entre clientes.

---

## Etapa 13 — Estrutura da solution e projeto de testes

Com o crescimento da aplicação, o repositório passou a possuir mais de um projeto .NET.

A estrutura foi reorganizada em:

```text
SaborSante/
├── SaborSante.slnx
├── src/
│   └── SaborSante.Api/
└── tests/
    └── SaborSante.Api.Tests/
```

Foi criada uma solution no formato `.slnx`, utilizado pelo .NET 10.

Conceitos estudados:

- solution;
- projetos `.csproj`;
- referência entre projetos;
- separação `src/` e `tests/`;
- dependência unidirecional entre projeto de testes e produção.

---

## Etapa 14 — Primeiros testes unitários

Foi introduzido **xUnit**.

O primeiro teste simples serviu apenas para comprovar que a infraestrutura estava funcionando.

Depois começaram os testes reais dos services.

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

## Etapa 15 — Interfaces motivadas por testabilidade

Ao tentar testar `ClienteService`, surgiu um problema concreto:

```text
ClienteService
      ↓
ClienteRepository
      ↓
NpgsqlDataSource
      ↓
PostgreSQL
```

O service estava diretamente acoplado à implementação concreta de persistência.

O primeiro teste precisou temporariamente utilizar:

```csharp
new ClienteService(null!)
```

Isso evidenciou a dificuldade de substituir a dependência.

Foi criada:

```text
IClienteRepository
```

e a estrutura passou a ser:

```text
ClienteService
      ↓
IClienteRepository
      ↑
ClienteRepository
```

Posteriormente, ao testar `ClienteEnderecoService`, apareceu o mesmo problema com a persistência de endereços.

Foi então criada:

```text
IClienteEnderecoRepository
```

A estrutura atual do service de endereços é:

```text
ClienteEnderecoService
        ├── IClienteRepository
        └── IClienteEnderecoRepository
```

As interfaces não foram criadas antecipadamente. Elas surgiram quando o acoplamento começou a impedir testes isolados.

Conceitos estudados:

- interfaces;
- contratos;
- implementações concretas;
- substituição de dependências;
- inversão de dependência aplicada na prática;
- redução de acoplamento;
- Dependency Injection através de abstrações.

O Dependency Inversion Principle ainda poderá ser estudado formalmente em uma etapa posterior. Neste momento, o conceito surgiu primeiro através de um problema concreto.

---

## Etapa 16 — Fakes manuais

Antes de utilizar bibliotecas de mocks, foram criados fakes manualmente:

```text
FakeClienteRepository
FakeClienteEnderecoRepository
```

Nos testes:

```text
ClienteService
      ↓
IClienteRepository
      ↑
FakeClienteRepository
```

e:

```text
ClienteEnderecoService
        ↓
IClienteEnderecoRepository
        ↑
FakeClienteEnderecoRepository
```

Os fakes permitem:

- configurar resultados;
- simular recursos encontrados ou inexistentes;
- registrar argumentos recebidos;
- contar quantidade de chamadas;
- verificar IDs enviados pelo service;
- testar sem PostgreSQL.

Exemplo conceitual:

```csharp
repository.ClienteParaRetornar = cliente;
```

ou:

```csharp
repository.ClienteParaRetornar = null;
```

Também foram utilizados contadores como:

```text
QuantidadeChamadasCriarAsync
QuantidadeChamadasAtualizarAsync
QuantidadeChamadasExcluirAsync
QuantidadeChamadasReativarAsync
```

Conceitos estudados:

- test doubles;
- fake manual;
- comportamento configurável;
- estado configurável;
- `Task.FromResult`;
- object initializers;
- verificação de estado;
- verificação de interação;
- verificação de argumentos;
- quantidade de chamadas.

Nenhuma biblioteca externa de mocks foi introduzida até o momento.

---

## Etapa 17 — Testes unitários do ClienteService

O `ClienteService` possui testes cobrindo suas principais operações.

### Criação

São testados:

- nome obrigatório;
- telefone obrigatório;
- nome `null`;
- telefone `null`;
- telefone sem dígitos;
- normalização de nome;
- normalização de telefone;
- criação válida;
- conflito de telefone;
- repository não chamado em validação inválida;
- repository chamado uma única vez em cenário válido;
- valores normalizados enviados à persistência.

### Atualização

São testados:

- atualização bem-sucedida;
- cliente não encontrado;
- conflito de telefone;
- normalização;
- validação;
- repository não chamado com dados inválidos;
- repository chamado uma única vez em cenário válido;
- argumentos enviados ao repository.

### Exclusão

A exclusão de cliente foi trazida para o mesmo fluxo dos demais comportamentos:

```text
ClienteEndpoints
      ↓
ClienteService
      ↓
IClienteRepository
```

São testados:

- exclusão/inativação bem-sucedida;
- nenhuma linha inativada;
- quantidade de chamadas;
- ID enviado ao repository.

### Reativação

São testados:

- reativação bem-sucedida;
- cliente não encontrado;
- cliente já ativo;
- conflito de telefone;
- quantidade de chamadas;
- ID enviado ao repository.

---

## Etapa 18 — Testes unitários do ClienteEnderecoService

O `ClienteEnderecoService` também passou a possuir testes isolados.

### Criação

São testados:

- cliente inexistente;
- criação bem-sucedida;
- normalização;
- conversão de campos opcionais vazios para `null`;
- identificação obrigatória;
- logradouro obrigatório;
- número obrigatório;
- bairro obrigatório;
- cidade obrigatória;
- repository não chamado em validações inválidas;
- repository chamado uma vez no fluxo válido;
- argumentos enviados à persistência.

### Atualização

São testados:

- atualização bem-sucedida;
- endereço não encontrado;
- cliente não encontrado;
- normalização;
- campos opcionais vazios convertidos para `null`;
- identificação obrigatória;
- logradouro obrigatório;
- número obrigatório;
- bairro obrigatório;
- cidade obrigatória;
- repository não chamado quando o fluxo é inválido;
- repository chamado uma vez no cenário válido;
- IDs e argumentos enviados ao repository.

### Exclusão

São testados:

- exclusão/inativação bem-sucedida;
- endereço não encontrado;
- cliente não encontrado;
- repository não chamado quando o cliente não existe;
- quantidade de chamadas;
- `clienteId` enviado;
- `enderecoId` enviado.

### Reativação

Os estados atuais de reativação de endereço são:

```text
Reativado
NaoEncontrado
JaAtivo
```

São testados:

- reativação bem-sucedida;
- endereço não encontrado;
- endereço já ativo;
- cliente não encontrado;
- repository não chamado quando o cliente não existe;
- quantidade de chamadas;
- IDs enviados ao repository.

Não existe atualmente um estado `Conflito` específico em `ResultadoReativacaoEndereco`.

---

# Arquitetura atual

A arquitetura continua propositalmente simples:

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

Para clientes:

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

Para endereços:

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

e:

```text
ClienteEnderecoServiceTests
        ↓
ClienteEnderecoService
        ├── FakeClienteRepository
        └── FakeClienteEnderecoRepository
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

O estado ativo/inativo é armazenado no PostgreSQL para suportar o ciclo de vida e o soft delete, mas não faz parte do record `Cliente`.

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
- telefone deve ser único entre clientes ativos;
- clientes podem ser inativados;
- clientes podem ser reativados;
- reativação pode gerar conflito de telefone;
- clientes podem possuir múltiplos endereços;
- endereços pertencem a um cliente específico;
- endereços possuem ciclo ativo/inativo;
- endereços inativos podem ser reativados;
- recursos filhos não podem ser manipulados através de outro cliente;
- dados obrigatórios são validados antes de chegar à persistência;
- campos opcionais de endereço vazios são normalizados para `null`.

---

# Estratégia atual de testes

Neste momento o projeto possui principalmente testes unitários dos services.

Eles verificam:

```text
Estado
→ o resultado retornado está correto?
```

e também:

```text
Interação
→ a dependência foi chamada?
→ quantas vezes?
→ quais argumentos recebeu?
```

Exemplo:

```text
dados inválidos
→ repository chamado 0 vezes

dados válidos
→ repository chamado 1 vez
```

Os testes atuais isolam os services utilizando fakes.

Eles **não testam ainda**:

```text
Repository
↓
Npgsql
↓
SQL
↓
PostgreSQL
```

Portanto, uma falha real de SQL ou de integração com o banco poderia não ser detectada pelos testes unitários atuais.

Essa limitação justifica a próxima etapa de estudo.

---

# Próxima etapa — Testes de integração

O próximo problema a ser estudado é:

> Como verificar automaticamente que os repositories realmente funcionam com PostgreSQL?

A próxima evolução deverá testar algo semelhante a:

```text
ClienteRepository
      ↓
Npgsql
      ↓
PostgreSQL real
```

Essa etapa deverá introduzir gradualmente conceitos como:

- teste unitário vs teste de integração;
- banco de desenvolvimento vs banco de testes;
- preparação de dados;
- limpeza de dados;
- isolamento entre testes;
- repetibilidade;
- lifecycle de infraestrutura de testes;
- execução real de SQL.

Ferramentas como **Testcontainers** poderão ser avaliadas posteriormente, somente depois que o problema que resolvem estiver claro.

Após isso, uma etapa posterior poderá automatizar também o fluxo HTTP:

```text
HTTP
 ↓
Endpoint
 ↓
Service
 ↓
Repository
 ↓
PostgreSQL
```

---

# Fluxo de desenvolvimento adotado

O projeto procura seguir este ciclo:

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
Versionamento
```

Um exemplo concreto:

```text
ClienteService depende de ClienteRepository
      ↓
service difícil de testar isoladamente
      ↓
primeiro teste utiliza null!
      ↓
acoplamento fica evidente
      ↓
IClienteRepository é introduzido
      ↓
FakeClienteRepository é criado
      ↓
service passa a ser testável sem PostgreSQL
```

O mesmo processo ocorreu posteriormente com:

```text
ClienteEnderecoRepository
↓
IClienteEnderecoRepository
↓
FakeClienteEnderecoRepository
```

Essa evolução incremental representa a filosofia central do projeto.

---

# Próximas evoluções

Entre os assuntos planejados estão:

- testes de integração;
- testes automatizados de HTTP/API;
- melhorias graduais na infraestrutura de testes;
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

Esses recursos não serão introduzidos simplesmente porque são comuns no mercado.

Cada conceito deverá surgir de uma necessidade concreta da aplicação.

---

# Visão de domínio

A operação da Sabor Santè envolve mais do que um CRUD simples.

A visão de longo prazo é representar algo próximo de:

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

O objetivo técnico não é apenas chegar a uma aplicação funcional, mas compreender profundamente as decisões que fazem um sistema simples evoluir para uma aplicação backend profissional.
