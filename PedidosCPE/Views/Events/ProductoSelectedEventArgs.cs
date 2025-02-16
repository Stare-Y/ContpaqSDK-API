using ApplicationLayer.DTOs;

namespace PedidosCPE.Views.Events
{
    public class ProductoSelectedEventArgs : EventArgs
    {
        public IEnumerable<ProductoDTO>? ProductosSeleccionados { get; set; } = new List<ProductoDTO>();
    }   
}
