# Order API

![Venha conhecer o projeto Order.](cl3oal8w6000c79570tgf4k1k.jpg)

Este projeto é uma API avançada desenvolvida utilizando diversas ferramentas e boas práticas de programação. Ele envolve conceitos como injeção de dependência, generics, JWT, UnitOfWork, Docker, AutoMapper, Dapper, REST, Swagger, Bryct, entre outros.


## Funcionalidades Principais

O objetivo principal do projeto é lidar com a venda de produtos, incluindo o cadastro de clientes. O banco de dados foi estruturado no SQL Server Management Studio com as seguintes tabelas:

- Usuário: Para autenticação e acesso à API.
- Produto: Informações sobre os produtos disponíveis.
- Cliente: Cadastro de clientes.
- Pedido: Informações sobre os pedidos realizados.
- PedidoItem: Detalhes dos itens em cada pedido.

### Segurança

As senhas são criptografadas e não são armazenadas no banco de dados por questões de segurança.


## Estrutura do Projeto

O projeto está dividido em quatro partes interligadas:

- **Order.API:** Inicialização da aplicação e ponto de entrada da API.
- **Order.Domain:** Camada de domínio.
- **Order.Application:** Camada de aplicação, responsável por fornecer e receber dados da API.
- **Order.Infra:** Camada de infraestrutura, cuida do acesso ao banco de dados e interações externas.

### Nomenclatura

Todos os componentes, incluindo nomes de classes, projetos, tabelas e propriedades, foram nomeados em inglês, seguindo as melhores práticas de programação.


## Feedback

Sinta-se à vontade para explorar e dar feedback através de elogios, sugestões e críticas.


## Como Contribuir

Se você deseja contribuir para o desenvolvimento deste projeto, siga as etapas abaixo:

### Relatando Problemas

Se encontrar algum problema ou tiver sugestões de melhorias, por favor, abra uma "Issue". Antes de criar uma nova "Issue", verifique se o problema já não foi relatado por outra pessoa.

### Contribuindo com Código

Se você deseja contribuir com código, siga estas etapas:

1. Fork do repositório.
2. Crie uma nova branch para suas alterações: `git checkout -b nome-da-sua-branch`.
3. Faça as alterações desejadas e faça commit: `git commit -m "Descrição das alterações"`.
4. Faça push para a sua branch: `git push origin nome-da-sua-branch`.
5. Abra um Pull Request (PR) com uma descrição clara das alterações propostas.

Agradeço antecipadamente por suas contribuições!

## Estrelas

Peço encarecidamente, se puder, dar estrela para que o projeto fique em destaque e mostre as outras pessoas.

  
