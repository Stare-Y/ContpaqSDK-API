using Core.Domain.Entities.DTOs;

namespace PedidosCPE.Views.Events
{
    public class CteProvSelectedEventArgs : EventArgs
    {
        public ClienteProveedorDto? ClienteProveedorSeleccionado { get; set; } = new ();
    }
}
