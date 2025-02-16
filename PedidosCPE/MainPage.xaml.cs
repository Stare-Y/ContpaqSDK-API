using PedidosCPE.Views.Documentos;
using PedidosCPE.Views.Settings;

namespace PedidosCPE
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void BtnCrearPedido(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(CreateDocumentoView));
        }

        private async void BtnVerPedidosPendientes_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(DispatchDocumentosPendientesView));
        }

        private async void BtnSettings_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(EditSettingsView));
        }

    }
}
