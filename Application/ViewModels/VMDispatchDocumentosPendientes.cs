using Core.Application.ViewModels.Base;
using Core.Application.ViewModels.ObservableEntity;
using Core.Domain.Entities.DTOs;
using Core.Domain.Entities.SDK.Estructuras;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Core.Domain.Interfaces.Services.ApiServices.Movimientos;
using Core.Domain.Interfaces.Services.ApiServices.Productos;
using System.Collections.ObjectModel;

namespace Core.Application.ViewModels
{
    public class VMDispatchDocumentosPendientes : ViewModelBase
    {
        private readonly IDocumentoService _documentoService = null!;
        private readonly IMovimientoService _movimientoService = null!;
        private readonly IProductoService _productoService = null!;
        private ObservableCollection<DocumentoDto> _documentosPendientes = new();
        private DocumentoDto _documentoSeleccionado = null!;
        private List<MovimientoDto> _movimientos = new();
        private List<ProductoDto> _productos = new();
        private ObservableCollection<ViewProductoUnidades> _productosUnidades = new();

        public VMDispatchDocumentosPendientes(IDocumentoService documentoService, IMovimientoService movimientoService, IProductoService productoService)
        {
            _documentoService = documentoService;
            _movimientoService = movimientoService;
            _productoService = productoService;
        }

        public VMDispatchDocumentosPendientes() { }

        public ObservableCollection<DocumentoDto> DocumentosPendientes
        {
            get => _documentosPendientes;
            set
            {
                _documentosPendientes = value;
                OnPropertyChanged(nameof(DocumentosPendientes));
            }
        }

        public List<MovimientoDto> Movimientos
        {
            get => _movimientos;
        }

        public List<ProductoDto> Productos
        {
            get => _productos;
        }

        public ObservableCollection<ViewProductoUnidades> ProductosUnidades
        {
            get => _productosUnidades;
            set
            {
                _productosUnidades = value;
                OnPropertyChanged(nameof(ProductosUnidades));
            }
        }

        public async Task LoadDocumentosPendientes()
        {
            DocumentosPendientes = new ObservableCollection<DocumentoDto>(await _documentoService.GetPendientes());
            OnPropertyChanged(nameof(DocumentosPendientes));
            return;
        }

        public DocumentoDto DocumentoSeleccionado
        {
            get => _documentoSeleccionado;
            set
            {
                _documentoSeleccionado = value;
                OnPropertyChanged(nameof(DocumentoSeleccionado));
            }
        }

        public async Task FetchMovimientosAndProductos()
        {
            try
            {
                if (DocumentoSeleccionado == null)
                {
                    return;
                }

                _movimientos = new(await _movimientoService.GetByDcocumentoIdAsync(DocumentoSeleccionado.IdPostgres));

                if (_movimientos.Count == 0)
                {
                    throw new Exception("No se encontraron movimientos para el documento seleccionado, algo anda mal");
                }

                _productos = new(await _productoService.GetByCodigosAsync(_movimientos.Select(m => m.CodigoProducto).ToList()));
                _productosUnidades = new();

                foreach (var producto in _productos)
                {
                    var productMatch = _movimientos.First(m => m.CodigoProducto == producto.CCODIGOPRODUCTO);
                    _productosUnidades.Add(new ViewProductoUnidades { Producto = producto, Unidades = productMatch.Unidades, Surtidas = productMatch.Surtidas });
                }

                OnCollectionChanged(nameof(ProductosUnidades));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los movimientos y productos del documento ({ex.Message})", ex);
            }
        }

        public async Task SaveDocumentAndMovementsOnSDK()
        {
            //validate if all movements have units
            if (_movimientos.Any(m => m.Surtidas == 0))
            {
                throw new Exception("No se puede guardar un movimiento con 0 unidades");
            }
            DocumentoSeleccionado.Impreso = true;

            try
            {
                var resultDTO = await _documentoService.PostDocumentoSDK(_documentoSeleccionado, Movimientos);
                DocumentoSeleccionado.IdContpaqiSQL = resultDTO.Keys.First();
                DocumentoSeleccionado.Folio = resultDTO.Values.First();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al guardar el documento y movimientos en el SDK ({ex.Message})", ex);
            }

            try
            {
                await _documentoService.PutAsync(DocumentoSeleccionado);
                await LoadDocumentosPendientes();
                DocumentoSeleccionado = null!;
                _productosUnidades = new();
                OnCollectionChanged(nameof(ProductosUnidades));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el documento en la base de datos ({ex.Message})", ex);
            }
        }
    }
}
