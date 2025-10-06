using System;
using System.Data.SQLite;
using System.Globalization;
using System.Linq;

namespace CheckPoint1.Services;

public class AdoNetService
{
    private readonly string _connectionString;

    public AdoNetService()
    {
        // Conecta-se ao mesmo arquivo de banco de dados usado pelo Entity Framework
        _connectionString = "Data Source=loja.db";
    }

    // ========== UTILIDADES ==========

    private SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_connectionString);
    }

    public void TestarConexao()
    {
        Console.Clear();
        Console.WriteLine("=== TESTE DE CONEXÃO (ADO.NET) ===");
        using var connection = GetConnection();
        try
        {
            Console.WriteLine("Tentando conectar...");
            connection.Open();
            Console.WriteLine($"Conexão bem-sucedida!");
            Console.WriteLine($"Versão do Servidor: {connection.ServerVersion}");

            using var command = new SQLiteCommand("SELECT sqlite_version();", connection);
            var version = command.ExecuteScalar()?.ToString();
            Console.WriteLine($"Versão do SQLite: {version}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Falha na conexão: {ex.Message}");
        }
    }

    // ========== CONSULTAS COMPLEXAS ==========

    public void RelatorioVendasCompleto()
    {
        Console.Clear();
        Console.WriteLine("=== RELATÓRIO VENDAS COMPLETO (ADO.NET) ===");

        string sql = @"
            SELECT p.NumeroPedido, c.Nome, pr.Nome, pi.Quantidade, pi.PrecoUnitario, (pi.Quantidade * pi.PrecoUnitario) AS Subtotal, p.DataPedido
            FROM Pedidos p
            JOIN Clientes c ON p.ClienteId = c.Id
            JOIN PedidoItens pi ON p.Id = pi.PedidoId
            JOIN Produtos pr ON pi.ProdutoId = pr.Id
            ORDER BY p.DataPedido DESC, p.NumeroPedido;";

        using var connection = GetConnection();
        connection.Open();
        using var command = new SQLiteCommand(sql, connection);
        using var reader = command.ExecuteReader();

        string currentPedido = "";
        while (reader.Read())
        {
            string numeroPedido = reader.GetString(0);
            if (numeroPedido != currentPedido)
            {
                currentPedido = numeroPedido;
                Console.WriteLine("\n-------------------------------------------------");
                Console.WriteLine($"Pedido: {numeroPedido} | Cliente: {reader.GetString(1)} | Data: {reader.GetDateTime(6):dd/MM/yyyy}");
            }
            Console.WriteLine($"  -> Item: {reader.GetString(2)}, Qtd: {reader.GetInt32(3)}, Preço Un.: {reader.GetDecimal(4):C}, Subtotal: {reader.GetDecimal(5):C}");
        }
        Console.WriteLine("\n-------------------------------------------------");
    }

    public void FaturamentoPorCliente()
    {
        Console.Clear();
        Console.WriteLine("=== FATURAMENTO POR CLIENTE ===");

        string sql = @"
            SELECT c.Nome, COUNT(p.Id) AS TotalPedidos, SUM(p.ValorTotal) AS FaturamentoTotal, AVG(p.ValorTotal) AS TicketMedio
            FROM Clientes c
            LEFT JOIN Pedidos p ON c.Id = p.ClienteId
            GROUP BY c.Id, c.Nome
            ORDER BY FaturamentoTotal DESC;";

        using var connection = GetConnection();
        connection.Open();
        using var command = new SQLiteCommand(sql, connection);
        using var reader = command.ExecuteReader();

        Console.WriteLine("{0,-25} {1,-15} {2,20} {3,20}", "Cliente", "Total Pedidos", "Faturamento Total", "Ticket Médio");
        Console.WriteLine(new string('-', 85));
        while (reader.Read())
        {
            Console.WriteLine("{0,-25} {1,-15} {2,20:C} {3,20:C}",
                reader.GetString(0),
                reader.GetInt32(1),
                reader.IsDBNull(2) ? 0 : reader.GetDecimal(2),
                reader.IsDBNull(3) ? 0 : reader.GetDecimal(3));
        }
    }

    public void ProdutosSemVenda()
    {
        Console.Clear();
        Console.WriteLine("=== PRODUTOS SEM VENDAS ===");

        string sql = @"
            SELECT c.Nome, pr.Nome, pr.Preco, pr.Estoque, (pr.Preco * pr.Estoque) AS ValorParado
            FROM Produtos pr
            JOIN Categorias c ON pr.CategoriaId = c.Id
            LEFT JOIN PedidoItens pi ON pr.Id = pi.ProdutoId
            WHERE pi.Id IS NULL;";

        using var connection = GetConnection();
        connection.Open();
        using var command = new SQLiteCommand(sql, connection);
        using var reader = command.ExecuteReader();

        decimal valorTotalParado = 0;
        Console.WriteLine("{0,-15} {1,-30} {2,15} {3,15}", "Categoria", "Produto", "Estoque", "Valor Parado");
        Console.WriteLine(new string('-', 80));
        while (reader.Read())
        {
            decimal valorParado = reader.GetDecimal(4);
            valorTotalParado += valorParado;
            Console.WriteLine("{0,-15} {1,-30} {2,15} {3,15:C}",
                reader.GetString(0),
                reader.GetString(1),
                reader.GetInt32(3),
                valorParado);
        }
        Console.WriteLine(new string('-', 80));
        Console.WriteLine($"Valor total parado em estoque: {valorTotalParado:C}");
    }

    // ========== OPERAÇÕES DE DADOS ==========

    public void AtualizarEstoqueLote()
    {
        Console.Clear();
        Console.WriteLine("=== ATUALIZAR ESTOQUE EM LOTE POR CATEGORIA ===");

        // Primeiro, lista as categorias para o usuário escolher
        using (var connection = GetConnection())
        {
            connection.Open();
            using var command = new SQLiteCommand("SELECT Id, Nome FROM Categorias", connection);
            using var reader = command.ExecuteReader();
            Console.WriteLine("Categorias disponíveis:");
            while (reader.Read()) Console.WriteLine($"ID: {reader.GetInt32(0)} - {reader.GetString(1)}");
        }

        int categoriaId = LerInteiro("\nDigite o ID da categoria para ajustar o estoque: ");

        using (var connection = GetConnection())
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Busca todos os produtos da categoria selecionada
                var selectCmd = new SQLiteCommand("SELECT Id, Nome, Estoque FROM Produtos WHERE CategoriaId = @id", connection, transaction);
                selectCmd.Parameters.AddWithValue("@id", categoriaId);

                using var reader = selectCmd.ExecuteReader();
                var produtosParaAtualizar = new System.Collections.Generic.List<(int Id, int NovoEstoque)>();

                Console.WriteLine("\nDigite o novo valor de estoque para cada produto:");
                while (reader.Read())
                {
                    int produtoId = reader.GetInt32(0);
                    string produtoNome = reader.GetString(1);
                    int estoqueAtual = reader.GetInt32(2);
                    int novoEstoque = LerInteiro($" -> {produtoNome} (atual: {estoqueAtual}): ");
                    produtosParaAtualizar.Add((produtoId, novoEstoque));
                }
                reader.Close(); // Fecha o reader antes de executar outros comandos

                // Agora, atualiza o estoque de cada produto
                int registrosAfetados = 0;
                var updateCmd = new SQLiteCommand("UPDATE Produtos SET Estoque = @estoque WHERE Id = @id", connection, transaction);
                foreach (var (id, estoque) in produtosParaAtualizar)
                {
                    updateCmd.Parameters.Clear();
                    updateCmd.Parameters.AddWithValue("@estoque", estoque);
                    updateCmd.Parameters.AddWithValue("@id", id);
                    registrosAfetados += updateCmd.ExecuteNonQuery();
                }

                transaction.Commit();
                Console.WriteLine($"\nOperação concluída. {registrosAfetados} produto(s) foram atualizados.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Console.WriteLine($"\nOcorreu um erro: {ex.Message}. A transação foi revertida.");
            }
        }
    }

    public void InserirPedidoCompleto()
    {
        Console.Clear();
        Console.WriteLine("=== INSERIR PEDIDO COMPLETO (ADO.NET) ===");

        int clienteId = LerInteiro("Digite o ID do cliente para o pedido: ");

        using var connection = GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            // 1. Inserir o pedido "master" e obter o ID
            var pedidoCmd = new SQLiteCommand(@"
                INSERT INTO Pedidos (NumeroPedido, DataPedido, Status, ValorTotal, ClienteId) 
                VALUES (@num, @data, @status, 0, @clienteId);
                SELECT last_insert_rowid();", connection, transaction);

            pedidoCmd.Parameters.AddWithValue("@num", Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper());
            pedidoCmd.Parameters.AddWithValue("@data", DateTime.UtcNow);
            pedidoCmd.Parameters.AddWithValue("@status", 1); // Pendente
            pedidoCmd.Parameters.AddWithValue("@clienteId", clienteId);

            long pedidoId = (long)pedidoCmd.ExecuteScalar();
            decimal valorTotalPedido = 0;

            // 2. Adicionar itens ao pedido
            bool adicionarMaisItens = true;
            while (adicionarMaisItens)
            {
                int produtoId = LerInteiro("\nDigite o ID do produto para adicionar (ou 0 para finalizar): ");
                if (produtoId == 0) break;

                int quantidade = LerInteiro("Digite a quantidade: ");

                // 3. Validar estoque e obter preço
                var produtoCmd = new SQLiteCommand("SELECT Estoque, Preco FROM Produtos WHERE Id = @id", connection, transaction);
                produtoCmd.Parameters.AddWithValue("@id", produtoId);
                using var reader = produtoCmd.ExecuteReader();

                if (!reader.Read())
                {
                    Console.WriteLine("Produto não encontrado.");
                    continue;
                }
                int estoqueAtual = reader.GetInt32(0);
                decimal precoUnitario = reader.GetDecimal(1);
                reader.Close();

                if (estoqueAtual < quantidade)
                {
                    Console.WriteLine($"Estoque insuficiente. Disponível: {estoqueAtual}.");
                    continue;
                }

                // 4. Inserir o item do pedido
                var itemCmd = new SQLiteCommand(@"
                    INSERT INTO PedidoItens (Quantidade, PrecoUnitario, PedidoId, ProdutoId) 
                    VALUES (@qtd, @preco, @pedidoId, @produtoId)", connection, transaction);
                itemCmd.Parameters.AddWithValue("@qtd", quantidade);
                itemCmd.Parameters.AddWithValue("@preco", precoUnitario);
                itemCmd.Parameters.AddWithValue("@pedidoId", pedidoId);
                itemCmd.Parameters.AddWithValue("@produtoId", produtoId);
                itemCmd.ExecuteNonQuery();

                // 5. Atualizar estoque
                var updateEstoqueCmd = new SQLiteCommand("UPDATE Produtos SET Estoque = Estoque - @qtd WHERE Id = @id", connection, transaction);
                updateEstoqueCmd.Parameters.AddWithValue("@qtd", quantidade);
                updateEstoqueCmd.Parameters.AddWithValue("@id", produtoId);
                updateEstoqueCmd.ExecuteNonQuery();

                valorTotalPedido += quantidade * precoUnitario;
                Console.WriteLine("Item adicionado!");
            }

            // 6. Atualizar o valor total do pedido "master"
            var updatePedidoCmd = new SQLiteCommand("UPDATE Pedidos SET ValorTotal = @total WHERE Id = @id", connection, transaction);
            updatePedidoCmd.Parameters.AddWithValue("@total", valorTotalPedido);
            updatePedidoCmd.Parameters.AddWithValue("@id", pedidoId);
            updatePedidoCmd.ExecuteNonQuery();

            transaction.Commit();
            Console.WriteLine($"\nPedido ID {pedidoId} criado com sucesso no valor de {valorTotalPedido:C}!");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"\nOcorreu um erro: {ex.Message}. A transação foi revertida.");
        }
    }

    public void ExcluirDadosAntigos()
    {
        Console.Clear();
        Console.WriteLine("=== EXCLUIR PEDIDOS CANCELADOS HÁ MAIS DE 6 MESES ===");

        var dataLimite = DateTime.UtcNow.AddMonths(-6);
        string sql = "DELETE FROM Pedidos WHERE Status = @status AND DataPedido < @dataLimite;";

        using var connection = GetConnection();
        connection.Open();
        using var command = new SQLiteCommand(sql, connection);
        command.Parameters.AddWithValue("@status", 5); // Cancelado
        command.Parameters.AddWithValue("@dataLimite", dataLimite);

        int rowsAffected = command.ExecuteNonQuery();
        Console.WriteLine($"{rowsAffected} pedido(s) antigos foram excluídos.");
    }

    public void ProcessarDevolucao()
    {
        Console.Clear();
        Console.WriteLine("=== PROCESSAR DEVOLUÇÃO DE PEDIDO ===");

        string numeroPedido = Console.ReadLine() ?? "";

        using var connection = GetConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();
        try
        {
            // 1. Localizar o pedido
            var pedidoCmd = new SQLiteCommand("SELECT Id, Status FROM Pedidos WHERE NumeroPedido = @num", connection, transaction);
            pedidoCmd.Parameters.AddWithValue("@num", numeroPedido);
            using var pedidoReader = pedidoCmd.ExecuteReader();
            if (!pedidoReader.Read())
            {
                Console.WriteLine("Pedido não encontrado.");
                transaction.Rollback();
                return;
            }
            long pedidoId = pedidoReader.GetInt64(0);
            int status = pedidoReader.GetInt32(1);
            pedidoReader.Close();

            // 2. Validar se pode devolver (ex: só se estiver Entregue)
            if (status != 4) // Entregue
            {
                Console.WriteLine($"Não é possível devolver um pedido com status diferente de 'Entregue'.");
                transaction.Rollback();
                return;
            }

            // 3. Devolver estoque
            var itensCmd = new SQLiteCommand("SELECT ProdutoId, Quantidade FROM PedidoItens WHERE PedidoId = @id", connection, transaction);
            itensCmd.Parameters.AddWithValue("@id", pedidoId);
            using var itensReader = itensCmd.ExecuteReader();
            var itensParaDevolver = new System.Collections.Generic.List<(int ProdutoId, int Quantidade)>();
            while (itensReader.Read())
            {
                itensParaDevolver.Add((itensReader.GetInt32(0), itensReader.GetInt32(1)));
            }
            itensReader.Close();

            var updateEstoqueCmd = new SQLiteCommand("UPDATE Produtos SET Estoque = Estoque + @qtd WHERE Id = @id", connection, transaction);
            foreach (var (produtoId, quantidade) in itensParaDevolver)
            {
                updateEstoqueCmd.Parameters.Clear();
                updateEstoqueCmd.Parameters.AddWithValue("@qtd", quantidade);
                updateEstoqueCmd.Parameters.AddWithValue("@id", produtoId);
                updateEstoqueCmd.ExecuteNonQuery();
            }

            // Opcional: Mudar status do pedido para 'Devolvido' (requer um novo status no Enum)
            // var updateStatusCmd = new SQLiteCommand("UPDATE Pedidos SET Status = @status WHERE Id = @id", connection, transaction); ...

            transaction.Commit();
            Console.WriteLine($"Devolução do pedido {numeroPedido} processada. Estoque atualizado.");
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"Ocorreu um erro: {ex.Message}. A transação foi revertida.");
        }
    }

    // ========== ANÁLISES PERFORMANCE ==========

    public void AnalisarPerformanceVendas()
    {
        Console.Clear();
        Console.WriteLine("=== ANÁLISE PERFORMANCE VENDAS ===");

        // SQLite não tem funções de data tão avançadas, então formatamos o mês/ano com strftime
        string sql = @"
            SELECT 
                strftime('%Y-%m', DataPedido) AS MesVenda,
                COUNT(Id) AS TotalPedidos,
                SUM(ValorTotal) AS Faturamento
            FROM Pedidos
            WHERE Status != 5 -- Exclui Cancelados
            GROUP BY MesVenda
            ORDER BY MesVenda DESC;";

        using var connection = GetConnection();
        connection.Open();
        using var command = new SQLiteCommand(sql, connection);
        using var reader = command.ExecuteReader();

        Console.WriteLine("{0,-10} {1,-15} {2,20}", "Mês/Ano", "Total Pedidos", "Faturamento");
        Console.WriteLine(new string('-', 50));
        while (reader.Read())
        {
            Console.WriteLine("{0,-10} {1,-15} {2,20:C}",
                reader.GetString(0),
                reader.GetInt32(1),
                reader.GetDecimal(2));
        }
    }

    // ========== MÉTODOS AUXILIARES ==========
    private int LerInteiro(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int resultado))
            {
                return resultado;
            }
            Console.WriteLine("Entrada inválida. Por favor, digite um número inteiro.");
        }
    }
}