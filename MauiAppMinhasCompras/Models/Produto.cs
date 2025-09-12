using SQLite;
namespace MauiAppMinhasCompras.Models
{
    public class Produto
    {
        string _descricao = "";
        double _quantidade = 0;
        double _preco = 0;
        string _categoria = "";

        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Descricao
        {
            get => _descricao;
            set => _descricao = value ?? "";
        }

        public double Quantidade
        {
            get => _quantidade;
            set => _quantidade = value;
        }

        public double Preco
        {
            get => _preco;
            set => _preco = value;
        }

        public string Categoria
        {
            get => _categoria;
            set => _categoria = value ?? "";
        }

        public double Total { get => Quantidade * Preco; }

        // Método para validar quando necessário (chame antes de salvar)
        public bool EhValido(out string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(Descricao))
            {
                mensagemErro = "Por favor, preencha a descrição.";
                return false;
            }

            if (Quantidade <= 0)
            {
                mensagemErro = "Por favor, preencha uma quantidade maior que 0.";
                return false;
            }

            if (Preco <= 0)
            {
                mensagemErro = "Por favor, preencha um preço maior que 0.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Categoria))
            {
                mensagemErro = "Por favor, preencha a categoria.";
                return false;
            }

            mensagemErro = "";
            return true;
        }
    }
}