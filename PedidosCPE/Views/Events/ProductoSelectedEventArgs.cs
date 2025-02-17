
using Core.Domain.Entities.DTOs;

namespace PedidosCPE.Views.Events
{
    public class ProductoSelectedEventArgs : EventArgs
    {
        public IEnumerable<ProductoDto>? ProductosSeleccionados { get; set; } = new List<ProductoDto>();
    }   
}
