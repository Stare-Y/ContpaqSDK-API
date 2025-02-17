using CommunityToolkit.Maui.Views;
using Core.Application.ViewModels;
using Core.Application.ViewModels.ObservableEntity;
using Core.Domain.Entities.DTOs;
using PedidosCPE.Views.Movimientos;

namespace PedidosCPE.Views.Documentos;

public partial class DispatchDocumentosPendientesView : ContentPage
{
	private readonly VMDispatchDocumentosPendientes _viewModel;
    private readonly IDispatcherTimer _timer;

    public DispatchDocumentosPendientesView(VMDispatchDocumentosPendientes viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = _viewModel;
        
        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(15);
        _timer.Tick += async (s, e) =>
        {
            try
            {
                if (_viewModel.ProductosUnidades.Count != 0)
                {
                    return;
                }
                await _viewModel.LoadDocumentosPendientes();
                await _viewModel.FetchMovimientosAndProductos();
            }
            catch (Exception ex)
            {
                BtnRefresh.IsVisible = true;
                _timer.Stop();
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        };
        _timer.Start();
    }
    public DispatchDocumentosPendientesView() : this(MauiProgram.ServiceProvider.GetRequiredService<VMDispatchDocumentosPendientes>())
    {
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(500);
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            await _viewModel.LoadDocumentosPendientes();
            await _viewModel.FetchMovimientosAndProductos();
        }
        catch (Exception ex)
        {
            if(_timer.IsRunning)
                _timer.Stop();
            await DisplayAlert("Error", ex.Message, "Ok");
        }
        finally
        {
            popup.Close();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _timer.Stop();
    }

    private async void productoSeleccionado_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (productoSeleccionado.SelectedItem == null)
        {
            return;
        }
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            var elementoSeleccionado = (ViewProductoUnidades)productoSeleccionado.SelectedItem;
            var movimiento = _viewModel.Movimientos.FirstOrDefault(m => m.CodigoProducto == elementoSeleccionado.Producto.CCODIGOPRODUCTO);
            if (movimiento == null)
            {
                throw new Exception("No se encontró el movimiento");
            }

            var capturar = new CaptureUnidadesView(elementoSeleccionado.Producto, movimiento);
            await Shell.Current.Navigation.PushAsync(capturar);
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

    private async void documentosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(documentosList.SelectedItem == null)
        {
            return;
        }
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            _viewModel.DocumentoSeleccionado = (DocumentoDto)documentosList.SelectedItem;
            await _viewModel.FetchMovimientosAndProductos();
            BtnCompletarDocumento.IsVisible = true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
            BtnCompletarDocumento.IsVisible = false;
        }
        finally
        {
            popup.Close();
        }
    }

    private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
        {
            documentosList.ItemsSource = _viewModel.DocumentosPendientes;
            return;
        }
    }

    private async void BtnCompletarDocumento_Clicked(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            var confirm = await DisplayAlert("Confirmar", $"¿Estás seguro de completar el pedido de {_viewModel.DocumentoSeleccionado.RazonSocial}?", "Si", "No");
            if (!confirm)
            {
                return;
            }
            await _viewModel.SaveDocumentAndMovementsOnSDK();
            BtnCompletarDocumento.IsVisible = false;
            //todo: then print a ticket
            await DisplayAlert("Éxito", "Documento enviado exitosamente :)", "Ok");
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

    private async void BtnRefresh_Clicked(object sender, EventArgs e)
    {
        var popup = new SpinnerPopup();
        this.ShowPopup(popup);
        try
        {
            await _viewModel.LoadDocumentosPendientes();
            await _viewModel.FetchMovimientosAndProductos();
            _timer.Start();
        }
        catch (Exception ex)
        {
            _timer.Stop();
            await DisplayAlert("Error", ex.Message, "Ok");
        }
        finally
        {
            BtnRefresh.IsVisible = false;
        }
    }
}