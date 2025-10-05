using CheckPoint1.Data;
using CheckPoint1.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace CheckPoint1.Services;

public class EntityFrameworkService : IDisposable
{
    private readonly CheckpointContext _context;

    public EntityFrameworkService()
    {
        _context = new CheckpointContext();
    }

    public async Task InicializarBancoDeDadosAsync()
    {
        await _context.Database.EnsureCreatedAsync();
    }

    // ========== CRUD CATEGORIAS ==========

    public void CriarCategoria()
    {
        Console.Clear();
        Console.WriteLine("=== CRIAR NOVA CATEGORIA ===");

        Console.Write("Digite o nome da nova categoria: ");
        string nome = Console.ReadLine() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("O nome da categoria не pode ser vazio. Operação cancelada.");
            return;
        }

        Console.Write("Digite a descrição (opcional): ");
        string? descricao = Console.ReadLine();

        var novaCategoria = new Categoria
        {
            Nome = nome,
            Descricao = descricao,
            DataCriacao = DateTime.UtcNow
        };

        try
        {
            _context.Categorias.Add(novaCategoria);
            _context.SaveChanges();
            Console.WriteLine($"\nCategoria '{novaCategoria.Nome}' criada com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nOcorreu um erro ao salvar a categoria: {ex.Message}");
        }
    }

    public void ListarCategorias()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE CATEGORIAS ===");

        var categorias = _context.Categorias
                                 .Include(c => c.Produtos)
                                 .ToList();

        if (!categorias.Any())
        {
            Console.WriteLine("Nenhuma categoria encontrada.");
            return;
        }

        Console.WriteLine("------------------------------------------");
        foreach (var categoria in categorias)
        {
            Console.WriteLine($"ID: {categoria.Id} | Nome: {categoria.Nome} | Produtos: {categoria.Produtos.Count}");
        }
        Console.WriteLine("------------------------------------------");
    }

    // ========== CRUD PRODUTOS ==========

    public void CriarProduto()
    {
        Console.Clear();
        Console.WriteLine("=== CRIAR NOVO PRODUTO ===");

        ListarCategorias();
        int categoriaId = LerInteiro("Digite o ID da categoria para o produto: ");

        if (!_context.Categorias.Any(c => c.Id == categoriaId))
        {
            Console.WriteLine("Categoria não encontrada. Operação cancelada.");
            return;
        }

        Console.Write("Digite o nome do produto: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("Digite a descrição (opcional): ");
        string? descricao = Console.ReadLine();

        decimal preco = LerDecimal("Digite o preço do produto (ex: 19.99): ");
        int estoque = LerInteiro("Digite a quantidade em estoque: ");

        var novoProduto = new Produto
        {
            Nome = nome,
            Descricao = descricao,
            Preco = preco,
            Estoque = estoque,
            DataCriacao = DateTime.UtcNow,
            Ativo = true,
            CategoriaId = categoriaId
        };

        _context.Produtos.Add(novoProduto);
        _context.SaveChanges();

        Console.WriteLine($"\nProduto '{novoProduto.Nome}' criado com sucesso!");
    }

    public void ListarProdutos()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE PRODUTOS ===");

        var produtos = _context.Produtos
                               .Include(p => p.Categoria)
                               .OrderBy(p => p.Nome)
                               .ToList();

        if (!produtos.Any())
        {
            Console.WriteLine("Nenhum produto encontrado.");
            return;
        }

        Console.WriteLine("-----------------------------------------------------------------------------");
        Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,10} {4,10}", "ID", "Nome", "Categoria", "Preço", "Estoque");
        Console.WriteLine("-----------------------------------------------------------------------------");
        foreach (var produto in produtos)
        {
            Console.WriteLine("{0,-5} {1,-30} {2,-15} {3,10:C} {4,10}",
                produto.Id,
                produto.Nome,
                produto.Categoria.Nome,
                produto.Preco,
                produto.Estoque);
        }
        Console.WriteLine("-----------------------------------------------------------------------------");
    }

    public void AtualizarProduto()
    {
        Console.Clear();
        ListarProdutos();
        int produtoId = LerInteiro("Digite o ID do produto que deseja atualizar: ");

        var produto = _context.Produtos.Find(produtoId);
        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado.");
            return;
        }

        Console.WriteLine($"Editando produto: {produto.Nome}");
        Console.Write($"Novo nome (atual: {produto.Nome}): ");
        string novoNome = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(novoNome)) produto.Nome = novoNome;

        Console.Write($"Nova descrição (atual: {produto.Descricao}): ");
        string novaDesc = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(novaDesc)) produto.Descricao = novaDesc;

        decimal novoPreco = LerDecimal($"Novo preço (atual: {produto.Preco:C}): ", true);
        if (novoPreco > 0) produto.Preco = novoPreco;

        int novoEstoque = LerInteiro($"Novo estoque (atual: {produto.Estoque}): ", true);
        if (novoEstoque >= 0) produto.Estoque = novoEstoque;

        _context.SaveChanges();
        Console.WriteLine("Produto atualizado com sucesso!");
    }

    // ========== CRUD CLIENTES ==========

    public void CriarCliente()
    {
        Console.Clear();
        Console.WriteLine("=== CRIAR NOVO CLIENTE ===");

        Console.Write("Nome completo: ");
        string nome = Console.ReadLine() ?? string.Empty;

        Console.Write("Email: ");
        string email = Console.ReadLine() ?? string.Empty;
        if (_context.Clientes.Any(c => c.Email == email))
        {
            Console.WriteLine("Este email já está cadastrado. Operação cancelada.");
            return;
        }

        Console.Write("CPF (somente números): ");
        string cpf = Console.ReadLine() ?? string.Empty;

        var novoCliente = new Cliente
        {
            Nome = nome,
            Email = email,
            CPF = new string(cpf.Where(char.IsDigit).ToArray()), // Armazena somente números
            DataCadastro = DateTime.UtcNow,
            Ativo = true
        };

        _context.Clientes.Add(novoCliente);
        _context.SaveChanges();
        Console.WriteLine($"Cliente '{novoCliente.Nome}' criado com sucesso!");
    }

    public void ListarClientes()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE CLIENTES ===");

        var clientes = _context.Clientes.Include(c => c.Pedidos).ToList();

        if (!clientes.Any())
        {
            Console.WriteLine("Nenhum cliente encontrado.");
            return;
        }

        Console.WriteLine("------------------------------------------------------------------");
        Console.WriteLine("{0,-5} {1,-25} {2,-25} {3,-10}", "ID", "Nome", "Email", "Pedidos");
        Console.WriteLine("------------------------------------------------------------------");
        foreach (var cliente in clientes)
        {
            Console.WriteLine("{0,-5} {1,-25} {2,-25} {3,-10}", cliente.Id, cliente.Nome, cliente.Email, cliente.Pedidos.Count);
        }
        Console.WriteLine("------------------------------------------------------------------");
    }

    public void AtualizarCliente()
    {
        ListarClientes();
        int clienteId = LerInteiro("Digite o ID do cliente que deseja atualizar: ");

        var cliente = _context.Clientes.Find(clienteId);
        if (cliente == null)
        {
            Console.WriteLine("Cliente não encontrado.");
            return;
        }

        Console.WriteLine($"Editando cliente: {cliente.Nome}");
        Console.Write($"Novo nome (atual: {cliente.Nome}): ");
        string novoNome = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(novoNome)) cliente.Nome = novoNome;


        _context.SaveChanges();
        Console.WriteLine("Cliente atualizado com sucesso!");
    }

    // ========== CRUD PEDIDOS ==========

    public void CriarPedido()
    {
        Console.Clear();
        Console.WriteLine("=== CRIAR NOVO PEDIDO ===");

        ListarClientes();
        int clienteId = LerInteiro("Digite o ID do cliente para o pedido: ");
        var cliente = _context.Clientes.Find(clienteId);
        if (cliente == null)
        {
            Console.WriteLine("Cliente não encontrado. Operação cancelada.");
            return;
        }

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var novoPedido = new Pedido
            {
                NumeroPedido = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                DataPedido = DateTime.UtcNow,
                Status = CheckPoint1.Enums.StatusPedido.Pendente,
                ClienteId = clienteId,
            };

            bool adicionarMaisItens = true;
            while (adicionarMaisItens)
            {
                Console.Clear();
                ListarProdutos();
                int produtoId = LerInteiro("\nDigite o ID do produto para adicionar ao pedido (ou 0 para finalizar): ");

                if (produtoId == 0)
                {
                    adicionarMaisItens = false;
                    continue;
                }

                var produto = _context.Produtos.Find(produtoId);
                if (produto == null)
                {
                    Console.WriteLine("Produto não encontrado.");
                    continue;
                }

                int quantidade = LerInteiro($"Digite a quantidade de '{produto.Nome}': ");

                if (produto.Estoque < quantidade)
                {
                    Console.WriteLine($"Estoque insuficiente. Disponível: {produto.Estoque}.");
                    continue;
                }

                produto.Estoque -= quantidade; // Abate do estoque

                var novoItem = new PedidoItem
                {
                    ProdutoId = produtoId,
                    Quantidade = quantidade,
                    PrecoUnitario = produto.Preco
                };
                novoPedido.Itens.Add(novoItem);
                Console.WriteLine("Item adicionado!");
                Thread.Sleep(1000);
            }

            if (!novoPedido.Itens.Any())
            {
                Console.WriteLine("Nenhum item adicionado. Pedido cancelado.");
                transaction.Rollback();
                return;
            }

            novoPedido.ValorTotal = novoPedido.Itens.Sum(i => i.Subtotal);

            _context.Pedidos.Add(novoPedido);
            _context.SaveChanges();
            transaction.Commit();

            Console.WriteLine($"\nPedido {novoPedido.NumeroPedido} criado com sucesso no valor de {novoPedido.ValorTotal:C}!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            transaction.Rollback();
        }
    }

    public void ListarPedidos()
    {
        Console.Clear();
        Console.WriteLine("=== LISTA DE PEDIDOS ===");
        var pedidos = _context.Pedidos
            .Include(p => p.Cliente)
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.DataPedido)
            .ToList();

        if (!pedidos.Any())
        {
            Console.WriteLine("Nenhum pedido encontrado.");
            return;
        }

        foreach (var pedido in pedidos)
        {
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine($"Pedido: {pedido.NumeroPedido} | Data: {pedido.DataPedido:dd/MM/yyyy} | Status: {pedido.Status}");
            Console.WriteLine($"Cliente: {pedido.Cliente.Nome} | Total: {pedido.ValorTotal:C}");
            Console.WriteLine("Itens:");
            foreach (var item in pedido.Itens)
            {
                Console.WriteLine($"  - {item.Produto.Nome} | Qtd: {item.Quantidade} | Preço Un.: {item.PrecoUnitario:C} | Subtotal: {item.Subtotal:C}");
            }
        }
        Console.WriteLine("------------------------------------------------------------------");
    }

    public void AtualizarStatusPedido()
    {
        Console.Clear();
        ListarPedidos();
        int pedidoId = LerInteiro("Digite o ID do pedido para atualizar o status: ");
        var pedido = _context.Pedidos.Find(pedidoId);
        if (pedido == null)
        {
            Console.WriteLine("Pedido não encontrado.");
            return;
        }

        Console.WriteLine($"Status atual: {pedido.Status}");
        Console.WriteLine("Escolha o novo status:");
        foreach (int i in Enum.GetValues(typeof(CheckPoint1.Enums.StatusPedido)))
        {
            Console.WriteLine($"{i} - {Enum.GetName(typeof(CheckPoint1.Enums.StatusPedido), i)}");
        }

        int novoStatusId = LerInteiro("Novo status: ");
        if (Enum.IsDefined(typeof(CheckPoint1.Enums.StatusPedido), novoStatusId))
        {
            pedido.Status = (CheckPoint1.Enums.StatusPedido)novoStatusId;
            _context.SaveChanges();
            Console.WriteLine("Status do pedido atualizado com sucesso!");
        }
        else
        {
            Console.WriteLine("Status inválido.");
        }
    }

    public void CancelarPedido()
    {
        Console.Clear();
        ListarPedidos();
        int pedidoId = LerInteiro("\nDigite o ID do pedido que deseja cancelar: ");

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var pedido = _context.Pedidos
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .FirstOrDefault(p => p.Id == pedidoId);

            if (pedido == null)
            {
                Console.WriteLine("Pedido não encontrado.");
                transaction.Rollback();
                return;
            }

            // Regra de negócio: só pode cancelar se estiver Pendente ou Confirmado
            if (pedido.Status != Enums.StatusPedido.Pendente && pedido.Status != Enums.StatusPedido.Confirmado)
            {
                Console.WriteLine($"Não é possível cancelar um pedido com status '{pedido.Status}'.");
                transaction.Rollback();
                return;
            }

            // Devolve os itens ao estoque
            foreach (var item in pedido.Itens)
            {
                item.Produto.Estoque += item.Quantidade;
                Console.WriteLine($"Devolvendo {item.Quantidade}x de '{item.Produto.Nome}' ao estoque.");
            }

            // Altera o status do pedido
            pedido.Status = Enums.StatusPedido.Cancelado;

            _context.SaveChanges();
            transaction.Commit();

            Console.WriteLine("\nPedido cancelado com sucesso e estoque atualizado!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocorreu um erro ao cancelar o pedido: {ex.Message}");
            transaction.Rollback();
        }
    }

    public void ConsultasAvancadas() => Console.WriteLine("Funcionalidade não implementada.");
    public void RelatoriosGerais() => Console.WriteLine("Funcionalidade не implementada.");

    public void Dispose()
    {
        _context?.Dispose();
    }

    // ========== MÉTODOS AUXILIARES ==========
    // Pequenos métodos para evitar repetição de código e tratar entradas do usuário.
    private int LerInteiro(string prompt, bool opcional = false)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine() ?? "";
            if (opcional && string.IsNullOrWhiteSpace(input)) return -1; // Retorna -1 se opcional e vazio

            if (int.TryParse(input, out int resultado))
            {
                return resultado;
            }
            Console.WriteLine("Entrada inválida. Por favor, digite um número inteiro.");
        }
    }

    private decimal LerDecimal(string prompt, bool opcional = false)
    {
        while (true)
        {
            Console.Write(prompt);
            string input = Console.ReadLine()?.Replace(',', '.') ?? "";
            if (opcional && string.IsNullOrWhiteSpace(input)) return -1; // Retorna -1 se opcional e vazio

            if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal resultado))
            {
                return resultado;
            }
            Console.WriteLine("Entrada inválida. Por favor, digite um número decimal (ex: 59.90).");
        }
    }
}