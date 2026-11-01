# Dados de demonstração

O ambiente local carrega este seed automaticamente depois da criação do schema SQL Server.

## Credenciais locais
- Login: `demo`
- Senha: `OrderDemo#2026!`

A senha é armazenada como hash BCrypt. Esta conta é exclusiva para demonstração local e não deve ser criada em produção.

## Dados
- 2 clientes fictícios com e-mails reservados em `.example.test`.
- 2 produtos com preços e estoque.
- 1 pedido com 2 unidades de teclado mecânico e estoque decrementado.

O script verifica a existência dos registros antes de inserir. Para reiniciar do zero, use `docker compose down -v` e depois `docker compose up --build`.
