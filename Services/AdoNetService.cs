using System.Data.SQLite;

namespace CheckPoint1.Services;

public class AdoNetService
{
    private readonly string _connectionString;
        
    public AdoNetService()
    {
        // TODO: Implementar connection string para SQLite 
        // Usar o mesmo arquivo "loja.db" criado pelo EF
        _connectionString = "";
    }
        
    // ========== CONSULTAS COMPLEXAS ==========
        
    public void RelatorioVendasCompleto()
    {
        // TODO: Implementar relatório com múltiplos JOINs 
        // SELECT com JOIN de 4 tabelas: Pedido, Cliente, PedidoItem, Produto
        // - Mostrar: NumeroPedido, NomeCliente, NomeProduto, Quantidade, PrecoUnitario, Subtotal
        // - Agrupar por pedido
        // - Ordenar por data do pedido
            
        Console.WriteLine("=== RELATÓRIO VENDAS COMPLETO (ADO.NET) ===");
    }
        
    public void FaturamentoPorCliente()
    {
        // TODO: Implementar consulta com GROUP BY e SUM 
        // - Agrupar por cliente
        // - Calcular valor total de pedidos
        // - Contar quantidade de pedidos
        // - Calcular ticket médio
        // - Ordenar por faturamento decrescente
            
        Console.WriteLine("=== FATURAMENTO POR CLIENTE ===");
    }
        
    public void ProdutosSemVenda()
    {
        // TODO: Implementar consulta com LEFT JOIN e IS NULL 
        // - Produtos que nunca foram vendidos
        // - Mostrar categoria, nome, preço, estoque
        // - Calcular valor parado em estoque
            
        Console.WriteLine("=== PRODUTOS SEM VENDAS ===");
    }
        
    // ========== OPERAÇÕES DE DADOS ==========
        
    public void AtualizarEstoqueLote()
    {
        // TODO: Implementar UPDATE em lote com  
        // - Solicitar categoria e percentual de ajuste
        // - Atualizar estoque de todos produtos da categoria (para cada categoria, exibir o produto e perguntar a quantidade que deve ter do produto)
        // - Exibir de quantos registros foram afetados
        
            
        Console.WriteLine("=== ATUALIZAR ESTOQUE EM LOTE ===");
    }
        
    public void InserirPedidoCompleto()
    {
        // TODO: Implementar INSERT com  
        // - Inserir pedido master - Pedido master é o que vai na tabela Pedidos.
        // - Inserir múltiplos itens - Pedido pode conter vários itens
        // - Atualizar estoque dos produtos
        // - Validar estoque antes de inserir o item no pedido
            
        Console.WriteLine("=== INSERIR PEDIDO COMPLETO ===");
    }
        
    public void ExcluirDadosAntigos()
    {
        // TODO: Implementar DELETE com subconsulta 
        // - Excluir pedidos cancelados há mais de 6 meses
            
        Console.WriteLine("=== EXCLUIR DADOS ANTIGOS ===");
    }
        
    public void ProcessarDevolucao()
    {
        // TODO: Implementar processo complexo 
        // 1. Localizar pedido e itens
        // 2. Validar se pode devolver
        // 3. Devolver estoque (no cadastro de produtos, aumentar o estoque de acordo com a quantidade do pedido.)
        
        
            
        Console.WriteLine("=== PROCESSAR DEVOLUÇÃO ===");
    }
        
    // ========== ANÁLISES PERFORMANCE ==========
        
    public void AnalisarPerformanceVendas()
    {
        // TODO: Implementar análise
        // - Vendas mensais
        // - Crescimento percentual
        
        
            
        Console.WriteLine("=== ANÁLISE PERFORMANCE VENDAS ===");
    }
        
    // ========== UTILIDADES ==========
        
    private SQLiteConnection GetConnection()
    {
        return new SQLiteConnection(_connectionString);
    }
        

    public void TestarConexao()
    {
        // TODO: Implementar teste de conexão 
        // - Tentar conectar
        // - Executar query simples
        // - Mostrar informações do banco
            
        Console.WriteLine("=== TESTE DE CONEXÃO ===");
    }
}