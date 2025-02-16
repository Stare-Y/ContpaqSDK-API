using ApplicationLayer.ViewModels;
using ApplicationLayer.ViewModels.BindableObjects;
using CommunityToolkit.Maui.Views;
using PedidosCPE.Views.ClientesProveedores;
using PedidosCPE.Views.Events;
using PedidosCPE.Views.Popups;
using PedidosCPE.Views.Productos;
using System.Collections.Specialized;

namespace PedidosCPE.Views.Documentos;

public partial class CreateDocumentoView : ContentPage
{
	private readonly VMCreateDocumento _viewModel;
    public CreateDocumentoView(VMCreateDocumento viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
	public CreateDocumentoView() : this(MauiProgram.ServiceProvider.GetRequiredService<VMCreateDocumento>())
    {
    }

    private async void OnProductoSelected(object? sender, ProductoSelectedEventArgs e)
    {
        if (e.ProductosSeleccionados != null)
        {
            foreach (var producto in e.ProductosSeleccionados)
            {
                //validate if the product is already in the list
                if (_viewModel.Productos.Any(p => p.Producto.CIDPRODUCTO == producto.CIDPRODUCTO))
                {
                    continue;
                }
                var productoUnidades = new ViewProductoUnidades
                {
                    Producto = producto,
                    Unidades = 0,
                    Surtidas = 0
                };
                _viewModel.Productos.Add(productoUnidades);
            }
            await Shell.Current.Navigation.PopAsync();
        }
    }

    private async void OnClienteProveedorSelected(object? sender, CteProvSelectedEventArgs e)
    {
        if (e.ClienteProveedorSeleccionado != null)
        {
            _viewModel.ClienteProveedorSeleccionado = e.ClienteProveedorSeleccionado;
            await Shell.Current.Navigation.PopAsync();
        }
    }

    private void DatePickerFecha_DateSelected(object sender, DateChangedEventArgs e)
    {
        if(e.NewDate < DateTime.Now)
        {
            DisplayAlert("Fecha Incorrecta", "La fecha no puede ser menor a la fecha actual", "Ok");
            DatePickerFecha.Date = DateTime.Now;
            return;
        }
        _viewModel.Documento.Fecha = e.NewDate.ToString("MM/dd/yyyy");
    }

    private async void BtnAgregarProductos_Clicked(object sender, EventArgs e)
    {
        var productos = new SearchProductosView();
        productos.ProductosSeleccionados += OnProductoSelected;
        await Shell.Current.Navigation.PushAsync(productos);
    }

    private async void BtnGuardarPedido_Clicked(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            await _viewModel.EnviarDocumentoMovimientos();
            await DisplayAlert("Guardado", "El documento ha sido guardado correctamente", "Ok");
            await Shell.Current.Navigation.PopAsync();
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

    private async void BtnSeleccionarSocio_Clicked(object sender, EventArgs e)
    {
        var clientes = new SearchClientesProveedoresView();
        clientes.ClienteProveedorSeleccionado += OnClienteProveedorSelected;
        await Shell.Current.Navigation.PushAsync(clientes);
    }

    private async void ListProductos_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ListProductos.SelectedItem == null)
        {
            return;
        }

        try
        {
            var productoSeleccionado = (ViewProductoUnidades)ListProductos.SelectedItem;
            if (productoSeleccionado != null)
            {
                var popup = new GetUnidadesPopup(productoSeleccionado);

                // Suscríbete al evento UnidadesCapturadas
                popup.UnidadesCapturadas += (s, eventArgs) =>
                {
                    if(eventArgs.Quitar)
                    {
                        _viewModel.Productos.Remove(productoSeleccionado);
                        return;
                    }
                    // Asigna el valor capturado a las unidades del producto
                    productoSeleccionado.Unidades = eventArgs.Unidades;

                    // Notifica el cambio en la colección
                    var index = _viewModel.Productos.IndexOf(productoSeleccionado);
                    if (index >= 0)
                    {
                        _viewModel.Productos[index] = productoSeleccionado; // Esto asegura que la UI se actualice.
                    }
                };

                // Muestra el popup
                await this.ShowPopupAsync(popup);

            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }
}