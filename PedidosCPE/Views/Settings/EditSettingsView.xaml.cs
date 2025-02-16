using ApplicationLayer.ViewModels;

namespace PedidosCPE.Views.Settings;

public partial class EditSettingsView : ContentPage
{
    private readonly VMEditSettings _vmEditSettings;
    public EditSettingsView()
	{
		InitializeComponent();
        _vmEditSettings = new VMEditSettings(Path.Combine(AppContext.BaseDirectory, "Data/SDKSettings.json"));
        BindingContext = _vmEditSettings;
        FiltrarClasif1.SelectedItem = BoolToString(_vmEditSettings.Settings.FiltrarClasif1);
        FiltrarClasif2.SelectedItem = BoolToString(_vmEditSettings.Settings.FiltrarClasif2);
        FiltrarClasif3.SelectedItem = BoolToString(_vmEditSettings.Settings.FiltrarClasif3);
        FiltrarClasif4.SelectedItem = BoolToString(_vmEditSettings.Settings.FiltrarClasif4);
        FiltrarClasif5.SelectedItem = BoolToString(_vmEditSettings.Settings.FiltrarClasif5);
        FiltrarClasif6.SelectedItem = BoolToString(_vmEditSettings.Settings.FiltrarClasif6);
    }

    private string BoolToString(bool x)
    {
        return x ? "true" : "false";
    }

    private void CodConcepto_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.CodConcepto = CodConcepto.Text;
    }

    private void Serie_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.Serie = Serie.Text;
    }

    private void CodigoCteProv_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.CodigoCteProv = CodigoCteProv.Text;
    }

    private void CodigoAlmacen_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.CodigoAlmacen = CodigoAlmacen.Text;
    }

    private void Referencia_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.Referencia = Referencia.Text;
    }

    private void ServerUri_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.ServerUri = ServerUri.Text;
    }

    private async void CIDVALORCLASIFICACION1_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            _vmEditSettings.Settings.CIDVALORCLASIFICACION1 = int.Parse(CIDVALORCLASIFICACION1.Text);
        }
        catch (Exception ex)
        {
            CIDVALORCLASIFICACION1.Text = e.OldTextValue;
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void CIDVALORCLASIFICACION2_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            _vmEditSettings.Settings.CIDVALORCLASIFICACION2 = int.Parse(CIDVALORCLASIFICACION2.Text);
        }
        catch (Exception ex)
        {
            CIDVALORCLASIFICACION2.Text = e.OldTextValue;
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void CIDVALORCLASIFICACION3_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            _vmEditSettings.Settings.CIDVALORCLASIFICACION3 = int.Parse(CIDVALORCLASIFICACION3.Text);
        }
        catch (Exception ex) 
        {
            CIDVALORCLASIFICACION3.Text = e.OldTextValue;
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void CIDVALORCLASIFICACION4_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            _vmEditSettings.Settings.CIDVALORCLASIFICACION4 = int.Parse(CIDVALORCLASIFICACION4.Text);
        }
        catch (Exception ex)
        {
            CIDVALORCLASIFICACION4.Text = e.OldTextValue;
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void CIDVALORCLASIFICACION5_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            _vmEditSettings.Settings.CIDVALORCLASIFICACION5 = int.Parse(CIDVALORCLASIFICACION5.Text);
        }
        catch (Exception ex)
        {
            CIDVALORCLASIFICACION5.Text = e.OldTextValue;
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void CIDVALORCLASIFICACION6_TextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            _vmEditSettings.Settings.CIDVALORCLASIFICACION6 = int.Parse(CIDVALORCLASIFICACION6.Text);
        }
        catch (Exception ex)
        {
            CIDVALORCLASIFICACION6.Text = e.OldTextValue;
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private void FiltrarClasif1Value_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltrarClasif2Value_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltrarClasif3Value_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltrarClasif4Value_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltrarClasif5Value_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void FiltrarClasif6Value_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void PuertoBascula_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.PuertoBascula = PuertoBascula.Text;
    }

    private void BaudRateBascula_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.BaudRateBascula = int.Parse(BaudRateBascula.Text);
    }

    private void DataBitsBascula_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.DataBitsBascula = int.Parse(DataBitsBascula.Text);
    }

    private void WriteCommandBascula_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.WriteCommandBascula = WriteCommandBascula.Text;
    }

    private void SufijoBascula_TextChanged(object sender, TextChangedEventArgs e)
    {
        _vmEditSettings.Settings.SufijoBascula = SufijoBascula.Text;
    }

    private void FiltrarClasif1_SelectedIndexChanged(object sender, EventArgs e)
    {
        _vmEditSettings.Settings.FiltrarClasif1 = StringToBool((string)FiltrarClasif1.SelectedItem);
    }

    private void FiltrarClasif2_SelectedIndexChanged(object sender, EventArgs e)
    {
        _vmEditSettings.Settings.FiltrarClasif2 = StringToBool((string)FiltrarClasif2.SelectedItem);
    }

    private void FiltrarClasif3_SelectedIndexChanged(object sender, EventArgs e)
    {
        _vmEditSettings.Settings.FiltrarClasif3 = StringToBool((string)FiltrarClasif3.SelectedItem);
    }

    private void FiltrarClasif4_SelectedIndexChanged(object sender, EventArgs e)
    {
        _vmEditSettings.Settings.FiltrarClasif4 = StringToBool((string)FiltrarClasif4.SelectedItem);
    }

    private void FiltrarClasif5_SelectedIndexChanged(object sender, EventArgs e)
    {
        _vmEditSettings.Settings.FiltrarClasif5 = StringToBool((string)FiltrarClasif5.SelectedItem);
    }

    private void FiltrarClasif6_SelectedIndexChanged(object sender, EventArgs e)
    {
        _vmEditSettings.Settings.FiltrarClasif6 = StringToBool((string)FiltrarClasif6.SelectedItem);
    }

    private bool StringToBool(string x)
    {
        return x == "true";
    }

    private async void BtnGuardar_Clicked(object sender, EventArgs e)
    {
        BtnGuardar.Opacity = 0;
        await BtnGuardar.FadeTo(1, 200);

        try
        {
            _vmEditSettings.SaveSettings();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "Ok");
        }
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        BtnCancelar.Opacity = 0;
        await BtnCancelar.FadeTo(1, 200);

        await Shell.Current.Navigation.PopAsync();
    }
}