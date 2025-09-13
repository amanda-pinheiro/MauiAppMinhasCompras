using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Helpers
{
    public class SQLiteDatabaseHelper
    {
        readonly SQLiteAsyncConnection _conn;

        public SQLiteDatabaseHelper(string path)
        {
            _conn = new SQLiteAsyncConnection(path);
            _conn.CreateTableAsync<Produto>().Wait();

        }

        public Task <int> Insert(Produto p) 
        {
            return _conn.InsertAsync(p);
        }
        public Task<List<Produto>> Update(Produto p)
        {
            string sql = "Update Produto SET Descricao=?, Quantidade=?, Categoria=?, Preco=? WHERE Id=?";

            return _conn.QueryAsync<Produto>(
                sql, p.Descricao, p.Quantidade, p.Categoria, p.Preco, p.Id
                );
        }
        public Task <int> Delete(int id) 
        {
            return _conn.Table<Produto>().DeleteAsync(i => i.Id == id);
        
        }

        public Task <List<Produto>> GetAll() 
        {
            return _conn.Table<Produto>().ToListAsync();        
        }
        
        public Task <List<Produto>> SearchAll(string q) 
        {
            string sql = "SELECT * FROM Produto WHERE Descricao LIKE '%"+ q + "%'";

            return _conn.QueryAsync<Produto>(sql);
        }

        // Alterações para o desafio 1
        public async Task<List<string>> GetCategorias()
        {
            try
            {
                var produtos = await _conn.Table<Produto>().ToListAsync();
                return produtos.Select(p => p.Categoria)
                              .Where(c => !string.IsNullOrWhiteSpace(c))
                              .Distinct()
                              .OrderBy(c => c)
                              .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar categorias: {ex.Message}");
            }
        }

        public Task<List<Produto>> GetByCategoria(string categoria)
        {
            string sql = "SELECT * FROM Produto WHERE Categoria = ?";
            return _conn.QueryAsync<Produto>(sql, categoria);
        }

    }
}
