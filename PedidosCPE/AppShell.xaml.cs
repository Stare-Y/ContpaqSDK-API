using PedidosCPE.Views.ClientesProveedores;
using PedidosCPE.Views.Documentos;
using PedidosCPE.Views.Movimientos;
using PedidosCPE.Views.Productos;
using PedidosCPE.Views.Settings;

namespace PedidosCPE
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //routing
            Routing.RegisterRoute(nameof(SearchProductosView), typeof(SearchProductosView));
            Routing.RegisterRoute(nameof(CreateDocumentoView), typeof(CreateDocumentoView));
            Routing.RegisterRoute(nameof(SearchClientesProveedoresView), typeof(SearchClientesProveedoresView));
            Routing.RegisterRoute(nameof(DispatchDocumentosPendientesView), typeof(DispatchDocumentosPendientesView));
            Routing.RegisterRoute(nameof(CaptureUnidadesView), typeof(CaptureUnidadesView));
            Routing.RegisterRoute(nameof(EditSettingsView), typeof(EditSettingsView));
        }
    }
}
