using CheckPoint1.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CheckPoint1;

public class Program
{
    private static readonly EntityFrameworkService EfService = new();
    private static readonly AdoNetService AdoService = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║      CHECKPOINT 1 - C# FIAP          ║");
        Console.WriteLine("║   Sistema de Gestão de Loja Online   ║");
        Console.WriteLine("║    Entity Framework + ADO.NET        ║");
        Console.WriteLine("╚══════════════════════════════════════╝");

        // Chama o método de inicialização do banco de dados
        await InicializarBanco();

        var continuar = true;
        while (continuar)
        {
            MostrarMenuPrincipal();
            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": MenuEntityFramework(); break;
                case "2": MenuAdoNet(); break;
                case "0": continuar = false; break;
                default:
                    Console.WriteLine("Opção inválida!");
                    Thread.Sleep(1000);
                    break;
            }

            if (continuar && opcao != "0")
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }

        EfService.Dispose();
        Console.WriteLine("Sistema encerrado!");
    }

    static async Task InicializarBanco()
    {
        try
        {
            Console.WriteLine("\nInicializando banco de dados...");
            // Chama o método no serviço que contém a lógica do .EnsureCreatedAsync()
            await EfService.InicializarBancoDeDadosAsync();
            Console.WriteLine("Banco de dados pronto para uso!");
            Thread.Sleep(1500); // Pausa para o usuário ver a mensagem
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro fatal ao inicializar o banco de dados: {ex.Message}");
            Console.WriteLine("O programa será encerrado.");
            Console.ReadKey();
            Environment.Exit(1); // Encerra a aplicação se o banco não puder ser iniciado
        }
    }

    static void MostrarMenuPrincipal()
    {
        Console.Clear();
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║           MENU PRINCIPAL             ║");
        Console.WriteLine("╠══════════════════════════════════════╣");
        Console.WriteLine("║ 1 - Entity Framework                 ║");
        Console.WriteLine("║ 2 - ADO.NET Direto                   ║");
        Console.WriteLine("║ 0 - Sair                             ║");
        Console.WriteLine("╚══════════════════════════════════════╝");
        Console.Write("Escolha uma opção: ");
    }

    static void MenuEntityFramework()
    {
        bool voltar = false;
        while (!voltar)
        {
            Console.Clear();
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║          ENTITY FRAMEWORK            ║");
            Console.WriteLine("╠══════════════════════════════════════╣");
            Console.WriteLine("║           CADASTROS                  ║");
            Console.WriteLine("║ 1 - Gerenciar Categorias             ║");
            Console.WriteLine("║ 2 - Gerenciar Produtos               ║");
            Console.WriteLine("║ 3 - Gerenciar Clientes               ║");
            Console.WriteLine("║ 4 - Gerenciar Pedidos                ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║           CONSULTAS                  ║");
            Console.WriteLine("║ 5 - Consultas LINQ Avançadas         ║");
            Console.WriteLine("║ 6 - Relatórios Gerais                ║");
            Console.WriteLine("║                                      ║");
            Console.WriteLine("║ 0 - Voltar ao Menu Principal         ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.Write("Escolha uma opção: ");

            var opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": MenuCategorias(); break;
                case "2": MenuProdutos(); break;
                case "3": MenuClientes(); break;
                case "4": MenuPedidos(); break;
                case "5": EfService.ConsultasAvancadas(); break;
                case "6": EfService.RelatoriosGerais(); break;
                case "0": voltar = true; break;
                default:
                    Console.WriteLine("Opção inválida!");
                    Thread.Sleep(1000);
                    break;
            }

            if (!voltar && opcao != "0")
            {
                Console.WriteLine("\nPressione qualquer tecla para continuar...");
                Console.ReadKey();
            }
        }
    }

    static void MenuCategorias()
    {
        Console.Clear();
        Console.WriteLine("--- CATEGORIAS ---");
        Console.WriteLine("1 - Criar Categoria");
        Console.WriteLine("2 - Listar Categorias");
        Console.Write("Escolha uma opção: ");
        var opcao = Console.ReadLine();
        switch (opcao)
        {
            case "1": EfService.CriarCategoria(); break;
            case "2": EfService.ListarCategorias(); break;
        }
    }

    static void MenuProdutos()
    {
        Console.Clear();
        Console.WriteLine("--- PRODUTOS ---");
        Console.WriteLine("1 - Criar Produto");
        Console.WriteLine("2 - Listar Produtos");
        Console.WriteLine("3 - Atualizar Produto");
        Console.Write("Escolha uma opção: ");
        var opcao = Console.ReadLine();
        switch (opcao)
        {
            case "1": EfService.CriarProduto(); break;
            case "2": EfService.ListarProdutos(); break;
            case "3": EfService.AtualizarProduto(); break;
        }
    }

    static void MenuClientes()
    {
        Console.Clear();
        Console.WriteLine("--- CLIENTES ---");
        Console.WriteLine("1 - Criar Cliente");
        Console.WriteLine("2 - Listar Clientes");
        Console.WriteLine("3 - Atualizar Cliente");
        Console.Write("Escolha uma opção: ");
        var opcao = Console.ReadLine();
        switch (opcao)
        {
            case "1": EfService.CriarCliente(); break;
            case "2": EfService.ListarClientes(); break;
            case "3": EfService.AtualizarCliente(); break;
        }
    }

    static void MenuPedidos()
    {
        Console.Clear();
        Console.WriteLine("--- PEDIDOS ---");
        Console.WriteLine("1 - Criar Pedido");
        Console.WriteLine("2 - Listar Pedidos");
        Console.WriteLine("3 - Atualizar Status");
        Console.WriteLine("4 - Cancelar Pedido");
        Console.Write("Escolha uma opção: ");
        var opcao = Console.ReadLine();
        switch (opcao)
        {
            case "1": EfService.CriarPedido(); break;
            case "2": EfService.ListarPedidos(); break;
            case "3": EfService.AtualizarStatusPedido(); break;
            case "4": EfService.CancelarPedido(); break;
        }
    }

    static void MenuAdoNet()
    {
        // Implementação do menu ADO.NET
        Console.Clear();
        Console.WriteLine("Menu ADO.NET ainda não implementado.");
    }
}