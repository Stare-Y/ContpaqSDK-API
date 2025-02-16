using ApplicationLayer.ViewModels;
using ApplicationLayer.ViewModels.BindableObjects;
using CommunityToolkit.Maui.Views;
using PedidosCPE.Views.Events;

namespace PedidosCPE.Views.Popups;

public partial class GetUnidadesPopup : Popup
{

	private readonly VMUnidadesPopup _viewModel;
    public event EventHandler<UnidadesConfirmedEventArgs>? UnidadesCapturadas;

    public GetUnidadesPopup(VMUnidadesPopup viewModel)
	{
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    public GetUnidadesPopup() : this(MauiProgram.ServiceProvider.GetRequiredService<VMUnidadesPopup>())
    {
    }

    public GetUnidadesPopup(ViewProductoUnidades productoUnidades) : this()
    {
        _viewModel.ProductoUnidades = productoUnidades;
        LblProductoName.Text = "Unidades para: " + productoUnidades.Producto.CNOMBREPRODUCTO;
    }

    private void EntryUnidades_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.NewTextValue))
        {
            LblError.IsVisible = false;
            return;
        }
        try
        {
            if (!double.TryParse(e.NewTextValue, out double unidades))
            {
                // Si no es válido, revertir al valor anterior
                ((Entry)sender).Text = e.OldTextValue;
                return;
            }
            _viewModel.ProductoUnidades.Unidades = double.Parse(e.NewTextValue);
        }
        catch (Exception ex)
        {
            LblError.IsVisible = true;
            LblError.Text = ex.Message;
        }
    }

    private void ButtonAceptar_Clicked(object sender, EventArgs e)
    {
        UnidadesCapturadas?.Invoke(this, new UnidadesConfirmedEventArgs{Unidades = _viewModel.ProductoUnidades.Unidades});
        this.Close();
    }

    private void ButtonQuitar_Clicked(object sender, EventArgs e)
    {
        UnidadesCapturadas?.Invoke(this, new UnidadesConfirmedEventArgs { Unidades = 0, Quitar = true});
        this.Close();
    }
}