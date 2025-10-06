# Sistema de Gestão de Loja Online (Console)

![C#](https://img.shields.io/badge/C%23-10.0-blueviolet?style=flat-square) ![.NET](https://img.shields.io/badge/.NET-8.0-blue?style=flat-square) ![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework-Core-green?style=flat-square)

## 🎯 Sobre o Projeto

Este é um sistema de gestão para uma Loja Online em formato de **aplicativo de console**, desenvolvido como um trabalho acadêmico. O projeto implementa todas as funcionalidades de backend necessárias para gerenciar um e-commerce, incluindo o ciclo completo de uma compra: cadastro de clientes e produtos, e criação de pedidos.

O projeto foi construído com uma arquitetura em camadas (Services, Data) e utiliza o **Entity Framework Core** para a persistência de dados, demonstrando os conceitos de um ORM em um ambiente .NET.

---

## ✨ Funcionalidades Principais

-   **Gerenciamento de Clientes:** CRUD completo para os usuários da loja.
-   **Gerenciamento de Produtos:** CRUD completo para o catálogo de produtos.
-   **Gerenciamento de Pedidos:** Sistema para criar um pedido, associá-lo a um cliente, adicionar múltiplos itens e calcular o valor total.
-   **Validações de Negócio:** Regras implementadas, como checagem de e-mail único para clientes e validação de estoque durante a criação de um pedido.
-   **Dados Iniciais (Seed Data):** A aplicação inicia com dados pré-carregados para facilitar os testes e a demonstração das funcionalidades.
-   **Interface de Console Interativa:** Todos os recursos são acessíveis através de um menu navegável no terminal.
-   **Persistência de Dados:** As informações são salvas em um banco de dados local SQLite, garantindo que os dados permaneçam entre as execuções.

---

## 🚀 Tecnologias Utilizadas

-   **Linguagem:** C# 10+
-   **Plataforma:** .NET 8
-   **ORM (Object-Relational Mapper):** Entity Framework Core
-   **Banco de Dados:** SQLite (arquivo local `loja.db`)
-   **Tipo de Projeto:** Aplicativo de Console

---

## ⚙️ Como Rodar o Projeto

1.  **Pré-requisitos:**
    * .NET SDK 8.0 ou superior.
    * Git (para clonar o repositório).

2.  **Clonar o Repositório:**
    ```bash
    git clone https://github.com/Akiozin/Desenvolvimento-de-software-em-CSharp-CP1.git
    cd Desenvolvimento-de-software-em-CSharp-CP1
    ```

3.  **Executar a Aplicação:**
    O comando `dotnet run` irá compilar e iniciar a aplicação. Na primeira execução, o Entity Framework Core criará automaticamente o banco de dados `loja.db` e o populará com os dados iniciais.
    ```bash
    dotnet run
    ```
    A aplicação iniciará no seu terminal, exibindo o menu principal.

---

## ⌨️ Como Usar a Aplicação

A interação com o sistema é feita através de menus no console. Após iniciar a aplicação, você verá o menu principal.

#### Exemplo de Fluxo de Compra:

1.  **Execute a aplicação:** `dotnet run`
2.  **Liste os Clientes:** Navegue até `1. Entity Framework > 3. Gerenciar Clientes > 2. Listar Clientes` para ver os clientes iniciais.
3.  **Crie um Novo Produto:** Navegue até `1. Entity Framework > 2. Gerenciar Produtos > 1. Criar Produto`. Siga as instruções para criar um novo item no catálogo.
4.  **Crie um Pedido:**
    * Vá para `1. Entity Framework > 4. Gerenciar Pedidos > 1. Criar Pedido`.
    * O sistema pedirá o ID de um cliente existente.
    * Em seguida, você poderá adicionar produtos ao pedido, informando o ID do produto e a quantidade.
    * Digite `0` quando terminar de adicionar produtos.
5.  **Verifique o Pedido:** Navegue até `1. Entity Framework > 4. Gerenciar Pedidos > 2. Listar Pedidos` para ver o pedido que você acabou de criar, com todos os detalhes.

---

## 🧪 Dados Iniciais

A aplicação inicia com alguns dados pré-carregados para facilitar os testes. Estes dados são inseridos via Entity Framework Core no método `OnModelCreating` da classe `CheckpointContext`.

* **Clientes Iniciais:**
    * `Carlos Andrade` (ID: 1)
    * `Ana Beatriz Costa` (ID: 2)
* **Produtos Iniciais:**
    * `Smartphone Pro X` (ID: 1)
    * `Notebook UltraSlim i7` (ID: 2)
    * `Livro: A Arte da Programação` (ID: 3)
    * `Camiseta Básica de Algodão` (ID: 4)

---

## 📝 Observações

#### Decisões de Implementação
-   **Banco de Dados SQLite:** Foi escolhido para simplificar a configuração e execução do projeto, não exigindo a instalação de um SGBD externo. O banco de dados é um arquivo local que persiste os dados.
-   **Interface de Console:** A escolha de um aplicativo de console foi para focar nos conceitos de C#, .NET, e na lógica de acesso a dados com Entity Framework Core, sem a complexidade de uma interface gráfica ou web.
-   **Transações:** Operações críticas, como a criação e o cancelamento de pedidos, são envolvidas em transações para garantir a consistência dos dados (ex: o estoque só é atualizado se o pedido for salvo com sucesso).

#### Limitações
-   **Interface de Usuário:** A interação é limitada a um menu de texto, sem elementos gráficos.
-   **Consultas e Relatórios:** As seções de "Consultas Avançadas" e "Relatórios" são placeholders e não foram implementadas.
-   **Autenticação:** Não há sistema de login; todas as operações são públicas.

#### Melhorias Futuras
-   Implementar as consultas LINQ avançadas e os relatórios gerais.
-   Adicionar validações mais robustas para as entradas do usuário.
-   Migrar a interface para uma tecnologia mais rica, como uma API REST com ASP.NET Core ou uma aplicação Desktop com MAUI/WPF.
-   Implementar um sistema de testes unitários para a camada de serviços.

## 👥 Integrantes

-   Guilherme Akio – RM: 98582
-   Guilherme Morais - RM: 55198
