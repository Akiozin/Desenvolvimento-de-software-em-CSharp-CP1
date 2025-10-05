using CheckPoint1.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace CheckPoint1.Data;

public class CheckpointContext : DbContext
{
    // As definições das tabelas
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Pedido> Pedidos { get; set; }
    public DbSet<PedidoItem> PedidoItens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=loja.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- SEED DE CATEGORIAS ---
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nome = "Eletrônicos", Descricao = "Dispositivos eletrônicos.", DataCriacao = DateTime.UtcNow },
            new Categoria { Id = 2, Nome = "Livros", Descricao = "Livros de diversos gêneros.", DataCriacao = DateTime.UtcNow },
            new Categoria { Id = 3, Nome = "Vestuário", Descricao = "Roupas, calçados e acessórios.", DataCriacao = DateTime.UtcNow }
        );

        // --- SEED DE PRODUTOS ---
        modelBuilder.Entity<Produto>().HasData(
            new Produto { Id = 1, Nome = "Smartphone Pro X", Descricao = "Smartphone com câmera tripla.", Preco = 3999.90m, Estoque = 50, DataCriacao = DateTime.UtcNow, Ativo = true, CategoriaId = 1 },
            new Produto { Id = 2, Nome = "Notebook UltraSlim i7", Descricao = "Notebook leve e potente.", Preco = 7499.00m, Estoque = 25, DataCriacao = DateTime.UtcNow, Ativo = true, CategoriaId = 1 },
            new Produto { Id = 3, Nome = "Livro: A Arte da Programação", Descricao = "Guia sobre algoritmos.", Preco = 129.99m, Estoque = 100, DataCriacao = DateTime.UtcNow, Ativo = true, CategoriaId = 2 },
            new Produto { Id = 4, Nome = "Camiseta Básica de Algodão", Descricao = "Camiseta para o dia a dia.", Preco = 59.90m, Estoque = 200, DataCriacao = DateTime.UtcNow, Ativo = true, CategoriaId = 3 }
        );

        // --- SEED DE CLIENTES ---
        modelBuilder.Entity<Cliente>().HasData(
            new Cliente { Id = 1, Nome = "Carlos Andrade", Email = "carlos.andrade@email.com", CPF = "12345678900", DataCadastro = DateTime.UtcNow, Ativo = true },
            new Cliente { Id = 2, Nome = "Ana Beatriz Costa", Email = "ana.costa@email.com", CPF = "09876543211", DataCadastro = DateTime.UtcNow, Ativo = true }
        );
    }
}