using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class Relatorio : ContentPage
{
	public Relatorio()
	{
        InitializeComponent();
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarRelatorio();
    }

    private async Task CarregarRelatorio()
    {
        try
        {
            lblInfo.Text = "Carregando dados...";

            // Limpar lista anterior
            stackCategorias.Children.Clear();

            // Buscar todos os produtos
            List<Produto> produtos = await App.Db.GetAll();

            if (produtos == null || produtos.Count == 0)
            {
                lblInfo.Text = "Nenhum produto encontrado";
                lblTotalGeral.Text = "Total Geral: R$ 0,00";
                return;
            }

            //Filtrar produtos que têm categoria
            var produtosComCategoria = produtos
                .Where(p => !string.IsNullOrWhiteSpace(p.Categoria))
                .ToList();

            if (produtosComCategoria.Count == 0)
            {
                lblInfo.Text = "Nenhum produto com categoria encontrado";
                lblTotalGeral.Text = "Total Geral: R$ 0,00";
                return;
            }

            // Agrupar por categoria e calcular totais
            var dadosAgrupados = produtosComCategoria
                .GroupBy(p => p.Categoria)
                .Select(g => new
                {
                    Categoria = g.Key,
                    Quantidade = g.Count(),
                    Total = g.Sum(p => p.Total)
                })
                .OrderByDescending(x => x.Total)
                .ToList();

            // Criar os cards para cada categoria
            foreach (var item in dadosAgrupados)
            {
                var frame = new Frame
                {
                    BackgroundColor = Colors.White,
                    BorderColor = Colors.LightGray,
                    CornerRadius = 10,
                    Padding = 15,
                    Margin = new Thickness(0, 5)
                };

                var stackLayout = new StackLayout();

                // Nome da categoria
                var lblCategoria = new Label
                {
                    Text = $"{item.Categoria}",
                    FontSize = 16,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.DarkBlue
                };

                // Quantidade de produtos
                var lblQuantidade = new Label
                {
                    Text = $"{item.Quantidade} produto{(item.Quantidade != 1 ? "s" : "")}",
                    FontSize = 14,
                    TextColor = Colors.Gray
                };

                // Total gasto
                var lblTotal = new Label
                {
                    Text = item.Total.ToString("C"),
                    FontSize = 18,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Green,
                    HorizontalOptions = LayoutOptions.End
                };

                stackLayout.Children.Add(lblCategoria);
                stackLayout.Children.Add(lblQuantidade);
                stackLayout.Children.Add(lblTotal);
                frame.Content = stackLayout;

                stackCategorias.Children.Add(frame);
            }

            // Calcular e mostrar total geral
            double totalGeral = dadosAgrupados.Sum(x => x.Total);
            lblTotalGeral.Text = $"Total Geral: {totalGeral:C}";

            // Mostrar informações
            lblInfo.Text = $"{dadosAgrupados.Count} categorias • {produtosComCategoria.Count} produtos";

        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao carregar relatório:\n{ex.Message}", "OK");
            lblInfo.Text = "Erro ao carregar dados";
            lblTotalGeral.Text = "Total Geral: R$ 0,00";
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await CarregarRelatorio();
    }
}