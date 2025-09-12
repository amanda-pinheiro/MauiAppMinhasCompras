using MauiAppMinhasCompras.Models;

namespace MauiAppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
	public EditarProduto()
	{
		InitializeComponent();
	}

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto produto_anexado = BindingContext as Produto;
            

            {
                produto_anexado.Descricao = txt_descricao.Text;
                produto_anexado.Quantidade = Convert.ToDouble(txt_quantidade.Text);
                produto_anexado.Preco = Convert.ToDouble(txt_preco.Text);
                produto_anexado.Categoria = txt_categoria.Text;
            }
            ;

            await App.Db.Update(produto_anexado);
            await DisplayAlert("Sucesso!", "Registro Atualizado", "OK");
            await Navigation.PopAsync();

        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}