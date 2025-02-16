using ApplicationLayer.DTOs;
using ApplicationLayer.ViewModels;
using CommunityToolkit.Maui.Views;
using PedidosCPE.Views.Events;

namespace PedidosCPE.Views.ClientesProveedores;

public partial class SearchClientesProveedoresView : ContentPage
{
    public event EventHandler<CteProvSelectedEventArgs>? ClienteProveedorSeleccionado;

    private readonly VMSearchClienteProveedor _viewModel;

    public SearchClientesProveedoresView(VMSearchClienteProveedor viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public SearchClientesProveedoresView() : this(MauiProgram.ServiceProvider.GetRequiredService<VMSearchClienteProveedor>())
    {
    }

    private async void searchBar_SearchButtonPressed(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            if (_viewModel.ClientesProveedoresEncontrados.Count > 0)
            {
                _viewModel.ClientesProveedoresEncontrados.Clear();
            }
            if (string.IsNullOrEmpty(searchBar.Text))
            {
                await DisplayAlert("Campo Vacío", "Por favor, ingrese algo para buscar", "Ok");
                return;
            }
            await _viewModel.SearchClienteProveedorAsync(searchBar.Text);

            if (_viewModel.ClientesProveedoresEncontrados.Count == 0)
            {
                await DisplayAlert("Sin Coincidencias :(", "No se encontraron clientes/proveedores con esa descripción", "Ok");
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

    private async void BtnSeleccionarCteProv_Clicked(object sender, EventArgs e)
    {
        try
        {
            var confirmacion = await DisplayAlert("Confirmación", $"¿Desea seleccionar al socio {_viewModel.ClienteProveedorSeleccionado.CRAZONSOCIAL}?", "Sí", "No");
            if (confirmacion)
            {
                ClienteProveedorSeleccionado?.Invoke(this, new CteProvSelectedEventArgs { ClienteProveedorSeleccionado = _viewModel.ClienteProveedorSeleccionado });
            }
            else
            {
                BtnSeleccionarCteProv.IsEnabled = false;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(searchBar.Text))
        {
            _viewModel.ClientesProveedoresEncontrados.Clear();
        }
    }

    private void resultList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        try
        {
            if (resultList.SelectedItem == null)
            {
                return;
            }
            ClienteProveedorDTO clienteProveedor = (ClienteProveedorDTO)resultList.SelectedItem ?? throw new Exception("No se seleccionó ningún cliente/proveedor");
            if (clienteProveedor != null)
            {
                _viewModel.ClienteProveedorSeleccionado = clienteProveedor;
                BtnSeleccionarCteProv.IsEnabled = true;
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "Ok");
        }
    }
}