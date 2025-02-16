using ApplicationLayer.DTOs;
using ApplicationLayer.ViewModels;
using CommunityToolkit.Maui.Views;
using PedidosCPE.Views.Events;

namespace PedidosCPE.Views.Productos;

public partial class SearchProductosView : ContentPage
{
	private readonly VMSearchProductos _viewModel;
    public event EventHandler<ProductoSelectedEventArgs>? ProductosSeleccionados;
    public SearchProductosView(VMSearchProductos viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public SearchProductosView() : this(MauiProgram.ServiceProvider.GetRequiredService<VMSearchProductos>())
    {
    }

    private async void searchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            if(_viewModel.ProductosEncontrados.Count > 0)
            {
                _viewModel.ProductosEncontrados.Clear();
            }

            await _viewModel.BuscarProductosPorNombre(searchBar.Text);

            if (_viewModel.ProductosEncontrados.Count == 0)
            {
                await DisplayAlert("Sin Coincidencias :(", "No se encontraron productos con esa descripcion", "Ok");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
        finally
        {
            popup.Close();
        }
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(searchBar.Text))
        {
            _viewModel.ProductosEncontrados.Clear();
        }
    }

    private async void resultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (resultList.SelectedItem == null)
            {
                return;
            }
            ProductoDTO producto = (ProductoDTO)resultList.SelectedItem ?? throw new Exception("No se seleccionó ningún producto");
            if (producto != null)
            {
                _viewModel.ProductosSeleccionados.Add(producto);
                _viewModel.ProductosEncontrados.Remove(producto);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private void selectedList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection != null)
        {
            BtnEliminarSeleccionado.IsEnabled = true;
        }
        else
        {
            BtnEliminarSeleccionado.IsEnabled = false;
        }
    }

    private void BtnEliminarSeleccionado_Clicked(object sender, EventArgs e)
    {
        if (selectedList.SelectedItem != null)
        {
            ProductoDTO producto = (ProductoDTO)selectedList.SelectedItem;
            _viewModel.EliminarProductoSeleccionado(producto);
            BtnEliminarSeleccionado.IsEnabled = false;
        }
    }

    private async void BtnConfirmarSeleccion_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool confirmacion = await DisplayAlert("Confirmar Selección", "¿Desea confirmar la selección de productos?", "Si", "No");
            if (confirmacion)
            {
                ProductosSeleccionados?.Invoke(this, new ProductoSelectedEventArgs { ProductosSeleccionados = _viewModel.ProductosSeleccionados });
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }
}