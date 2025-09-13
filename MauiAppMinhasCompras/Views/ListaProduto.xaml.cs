using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
    ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

    public ListaProduto()
    {
        InitializeComponent();
        lst_produtos.ItemsSource = lista;
    }

    protected override async void OnAppearing()
    {
        await CarregarDados();
        await CarregarCategorias(); // NOVO: Carregar categorias no picker
    }

    // NOVO MÉTODO: Carregar dados
    private async Task CarregarDados()
    {
        try
        {
            lista.Clear();
            List<Produto> tmp = await App.Db.GetAll();
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    // NOVO MÉTODO: Carregar categorias no Picker
    private async Task CarregarCategorias()
    {
        try
        {
            List<string> categorias = await App.Db.GetCategorias();

            // Adicionar opção "Todas as categorias" no início
            categorias.Insert(0, "Todas as categorias");

            // Definir as opções no Picker
            pickerCategoria.ItemsSource = categorias;
            pickerCategoria.SelectedIndex = 0; // Selecionar "Todas as categorias"
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Erro ao carregar categorias: {ex.Message}", "OK");
        }
    }

    // NOVO MÉTODO: Filtrar produtos por categoria
    private async Task FiltrarPorCategoria(string categoria)
    {
        try
        {
            lista.Clear();
            lst_produtos.IsRefreshing = true;

            List<Produto> produtos;

            if (categoria == "Todas as categorias")
            {
                // Mostrar todos os produtos
                produtos = await App.Db.GetAll();
            }
            else
            {
                // Mostrar apenas produtos da categoria selecionada
                produtos = await App.Db.GetByCategoria(categoria);
            }

            // Adicionar produtos à lista
            foreach (var produto in produtos)
            {
                lista.Add(produto);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Navigation.PushAsync(new Views.NovoProduto());
        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            string q = e.NewTextValue;
            lista.Clear();
            lst_produtos.IsRefreshing = true;
            List<Produto> tmp = await App.Db.SearchAll(q);
            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            double soma = lista.Sum(i => i.Total);
            string msg = $"O total é {soma:C}";
            await DisplayAlert("Total dos Produtos", msg, "OK"); // CORRIGIDO: adicionei await
        }
        catch (Exception ex)
        {
            await DisplayAlert("Total dos Produtos", ex.Message, "OK");
        }
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem; // CORRIGIDO: "selecionado"
            Produto p = selecionado.BindingContext as Produto;
            bool confirm = await DisplayAlert(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");
            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;
            if (p == null) return;
            await Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p
            });
            // Limpar a seleção
            ((ListView)sender).SelectedItem = null;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            await CarregarDados(); // MELHORADO: usa o método CarregarDados
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {
        try
        {
            await Navigation.PushAsync(new Views.Relatorio());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    // ATUALIZADO: Método do picker de categoria
    private async void pickerCategoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            var picker = sender as Picker;
            string categoriaSelecionada = picker?.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(categoriaSelecionada))
                return;

            await FiltrarPorCategoria(categoriaSelecionada);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }
}