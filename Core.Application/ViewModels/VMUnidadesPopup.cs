using Core.Application.ViewModels.ObservableEntity;

namespace Core.Application.ViewModels
{
    public class VMUnidadesPopup
    {
        private ViewProductoUnidades _productoUnidades = new();
        public ViewProductoUnidades ProductoUnidades
        {
            get => _productoUnidades;
            set
            {
                _productoUnidades = value;
            }
        }
        public VMUnidadesPopup() { }
        public VMUnidadesPopup(ViewProductoUnidades productoUnidades)
        {
            _productoUnidades = productoUnidades;
        }
    }
}
