# Order

Sistema de gerenciamento de clientes, produtos e pedidos, com API REST em ASP.NET Core, SQL Server e interface web planejada com Razor Pages.

## Tecnologias

- .NET 10 / ASP.NET Core
- C#
- SQL Server
- Dapper
- AutoMapper
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

### 2. Preparar o SQL Server

Copie o arquivo de exemplo e defina uma senha local forte:

```bash
cp .env.example .env
```

Edite `.env` e defina `MSSQL_SA_PASSWORD`. Não versione o arquivo `.env`.

Suba o banco:

```bash
docker compose up -d sqlserver
```

Aplique o esquema inicial:

```bash
set -a
source .env
set +a
docker exec -it order-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -C -i /scripts/schema.sql
```

O script cria o banco `OrderDb` e as tabelas de clientes, usuários, produtos, pedidos e itens de pedido.

### 3. Configurar os segredos da API

Os valores sensíveis são mantidos no .NET User Secrets durante o desenvolvimento, não no repositório:

```bash
set -a
source .env
set +a

dotnet user-secrets set "ConnectionStrings:default" "Server=localhost,1433;Database=OrderDb;User Id=sa;Password=$MSSQL_SA_PASSWORD;TrustServerCertificate=True;Encrypt=False" --project Order.API/Order.csproj

dotnet user-secrets set "AuthSettings:Secret" "Order-Development-Only-Signing-Key-2026-Do-Not-Reuse" --project Order.API/Order.csproj
```

Em ambientes publicados, configure `ConnectionStrings__default` e `AuthSettings__Secret` como variáveis de ambiente/segredos do provedor.

### 4. Restaurar, compilar e executar

```bash
dotnet restore Order.sln
dotnet build Order.sln --no-restore
dotnet run --project Order.API/Order.csproj
```

Swagger: [http://localhost:5005/swagger](http://localhost:5005/swagger).

## Banco de dados

O esquema inicial está em [scrip order.sql](scrip%20order.sql). Ele foi alinhado aos nomes de tabelas e tipos usados pelos repositórios. O banco local padrão é `OrderDb`.

## Segurança

As senhas dos usuários são armazenadas como hashes BCrypt. A chave JWT e a connection string não devem ser commitadas; use User Secrets localmente e o gerenciador de segredos do ambiente publicado.

## Estado do projeto

O projeto está em processo de recuperação e finalização da V1. As PRs de estabilização são mantidas abertas como rascunho até a compilação, os testes automatizados e a execução com SQL Server serem validados.
