using Core.Application.ViewModels.Base;
using Core.Domain.Entities;
using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices.Productos;
using System.Collections.ObjectModel;

namespace Core.Application.ViewModels
{
    public class VMSearchProductos : ViewModelBase
    {
        private readonly IProductoService _productosService = null!;
        private ObservableCollection<ProductoDto> _productosEncontrados = new();
        private ObservableCollection<ProductoDto> _productosSeleccionados = new();
        private readonly TerminalSettings _terminalSettings = null!;
        private readonly List<bool> _filterRequirements = new();
        private readonly List<int> _valoresClasificaciones = new();
        private readonly List<string> _accionesClasificaciones = new();
        public VMSearchProductos(IProductoService productosService, TerminalSettings terminalSettings)
        {
            _productosService = productosService;
            _terminalSettings = terminalSettings;

            //load
            _filterRequirements.Add(terminalSettings.FiltrarClasif1);
            _filterRequirements.Add(terminalSettings.FiltrarClasif2);
            _filterRequirements.Add(terminalSettings.FiltrarClasif3);
            _filterRequirements.Add(terminalSettings.FiltrarClasif4);
            _filterRequirements.Add(terminalSettings.FiltrarClasif5);
            _filterRequirements.Add(terminalSettings.FiltrarClasif6);
            _valoresClasificaciones.Add(terminalSettings.CIDVALORCLASIFICACION1);
            _valoresClasificaciones.Add(terminalSettings.CIDVALORCLASIFICACION2);
            _valoresClasificaciones.Add(terminalSettings.CIDVALORCLASIFICACION3);
            _valoresClasificaciones.Add(terminalSettings.CIDVALORCLASIFICACION4);
            _valoresClasificaciones.Add(terminalSettings.CIDVALORCLASIFICACION5);
            _valoresClasificaciones.Add(terminalSettings.CIDVALORCLASIFICACION6);
            _accionesClasificaciones.Add(terminalSettings.FiltrarClasif1Value);
            _accionesClasificaciones.Add(terminalSettings.FiltrarClasif2Value);
            _accionesClasificaciones.Add(terminalSettings.FiltrarClasif3Value);
            _accionesClasificaciones.Add(terminalSettings.FiltrarClasif4Value);
            _accionesClasificaciones.Add(terminalSettings.FiltrarClasif5Value);
            _accionesClasificaciones.Add(terminalSettings.FiltrarClasif6Value);
        }

        public VMSearchProductos() { }

        public ObservableCollection<ProductoDto> ProductosEncontrados
        {
            get => _productosEncontrados;
            set
            {
                _productosEncontrados = value;
                OnPropertyChanged(nameof(ProductosEncontrados));
            }
        }

        public ObservableCollection<ProductoDto> ProductosSeleccionados
        {
            get => _productosSeleccionados;
            set
            {
                _productosSeleccionados = value;
                OnPropertyChanged(nameof(ProductosSeleccionados));
            }
        }

        public async Task BuscarProductosPorNombre(string nombre)
        {
            var productos = await _productosService.SearchByNombreAsync(nombre);
            foreach (var producto in productos)
            {
                var needsFilter = FiltrarProducto(producto);
                if (needsFilter)
                    continue;
                if (!ProductosSeleccionados.Contains(producto))
                {
                    ProductosEncontrados.Add(producto);
                }
            }
            OnCollectionChanged(nameof(ProductosEncontrados));
        }

        private bool FiltrarProducto(ProductoDto producto)
        {
            var index = 0;
            foreach (var needFilter in _filterRequirements)
            {
                if (needFilter)
                {
                    return AccionFiltro(_accionesClasificaciones[index], _valoresClasificaciones[index], producto, index);
                }
                index++;
            }
            return false;
        }

        private bool AccionFiltro(string filtro, int valor, ProductoDto producto, int indice)
        {
            // Construimos el nombre de la propiedad basado en el índice
            string nombrePropiedad = $"CIDVALORCLASIFICACION{indice + 1}";

            // Reflection to get the property value
            var tipoProducto = producto.GetType();
            var propiedad = tipoProducto.GetProperty(nombrePropiedad);

            if (propiedad == null)
                throw new Exception($"La propiedad {nombrePropiedad} no existe en ProductoDTO.");

            var valorPropiedad = (int)propiedad.GetValue(producto);

            // Validate filter
            if (filtro == "ignore")
                throw new Exception("Incongruencia en la lógica de filtro de SDKSettings.json");

            //deben estar invertidos, think about it
            if (filtro == "equal")
                return valor != valorPropiedad;

            if (filtro == "not")
                return valor == valorPropiedad;

            throw new Exception($"Incongruencia en la lógica de filtro de SDKSettings.json, no se pudo aplicar ningún filtro. Filto solicitado:{filtro}");
        }


        public void EliminarProductoSeleccionado(ProductoDto producto)
        {
            ProductosSeleccionados.Remove(producto);
            OnCollectionChanged(nameof(ProductosSeleccionados));
        }
    }
}
