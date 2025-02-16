using ApplicationLayer.DTOs;

namespace PedidosCPE.Views.Events
{
    public class CteProvSelectedEventArgs : EventArgs
    {
        public ClienteProveedorDTO? ClienteProveedorSeleccionado { get; set; } = new ();
    }
}
