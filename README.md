# Order

Sistema de gerenciamento de clientes, produtos e pedidos, com API REST em ASP.NET Core, SQL Server e interface web em Razor Pages integrada à API REST.

## Tecnologias

- .NET 10 / ASP.NET Core
- C#
- SQL Server
- Dapper
- Mapeamento explícito de DTOs
- JWT Bearer
- Swagger / OpenAPI
- Razor Pages
- Docker

## Estrutura

- `Order.API`: API REST e composição da aplicação.
- `Order.Application`: casos de uso, contratos de entrada/saída e mapeamentos.
- `Order.Domain`: entidades, regras de negócio, validações e interfaces.
- `Order.Infra`: persistência SQL Server e repositórios.
- `Order`: aplicação web Razor Pages.

## Configuração local

### 1. Pré-requisitos

- .NET SDK 10
- Docker Desktop ou Docker Engine com Docker Compose
- SQL Server em execução (o Compose abaixo fornece um ambiente local)

### 2. Subir a aplicação completa com Docker Compose

Copie o arquivo de exemplo e configure os segredos locais:

```bash
cp .env.example .env
```

Edite `.env` com valores locais fortes. Não versione esse arquivo.

Suba SQL Server, inicialização automática do schema, API e interface Razor Pages:

```bash
docker compose up --build
```

O serviço de inicialização aguarda o SQL Server ficar saudável e aplica o schema antes de iniciar a API. Não é necessário executar comandos SQL manualmente.

- Interface web: [http://localhost:8080](http://localhost:8080)
- API / Swagger: [http://localhost:5005/swagger](http://localhost:5005/swagger)
- SQL Server: `localhost,1433`

Para encerrar:

```bash
docker compose down
```

Para apagar também o volume local do banco (isso remove os dados):

```bash
docker compose down -v
```


### 3. Executar sem Docker (opcional)

Se preferir executar os projetos diretamente com o SDK, configure SQL Server localmente e use User Secrets:

```bash
set -a
source .env
set +a

dotnet user-secrets set "ConnectionStrings:default" "Server=localhost,1433;Database=OrderDb;User Id=sa;Password=$MSSQL_SA_PASSWORD;TrustServerCertificate=True;Encrypt=False" --project Order.API/Order.csproj
dotnet user-secrets set "AuthSettings:Secret" "$AUTH_JWT_SECRET" --project Order.API/Order.csproj
dotnet user-secrets set "AuthSettings:Issuer" "Order.Api" --project Order.API/Order.csproj
```

Aplique o schema manualmente apenas nesse modo alternativo, caso seu SQL Server ainda não tenha sido inicializado. No fluxo Docker Compose, schema e dados de demonstração são preparados automaticamente.

Compile a solução:

```bash
dotnet restore Order.sln
dotnet build Order.sln --no-restore
```

Inicie a API e a interface em terminais separados:

```bash
dotnet run --project Order.API/Order.csproj
dotnet run --project Order/Order.csproj
```

A interface usa `OrderApi:BaseUrl` (padrão `http://localhost:5005/`). Crie uma conta pela página `/Register` ou use o usuário de demonstração carregado no ambiente Docker.

## Banco de dados

O esquema inicial está em [scrip order.sql](scrip%20order.sql). Ele foi alinhado aos nomes de tabelas e tipos usados pelos repositórios. O banco local padrão é `OrderDb`.

## Segurança

As senhas dos usuários são armazenadas como hashes BCrypt. A chave JWT e a connection string não devem ser commitadas; use User Secrets localmente e o gerenciador de segredos do ambiente publicado.

## Estado do projeto

A V1 está em validação. As PRs de estabilização permanecem como rascunho até a compilação, os testes automatizados e a execução ponta a ponta da API e da interface serem validados.


## Validation strategy

- Unit tests: `Order.Tests` (domain validation rules).
- SQL Server integration tests: `Order.IntegrationTests` (repository persistence against SQL Server 2022).
- CI: GitHub Actions restores and builds the complete .NET 10 solution, runs unit tests, and runs SQL Server integration tests in an ephemeral service container.
- A release is considered ready only after CI passes and the API plus Razor Pages have been manually exercised end-to-end.
