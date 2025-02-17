using Core.Application.ViewModels.Base;
using Core.Application.ViewModels.ObservableEntity;
using Core.Domain.Entities;
using Core.Domain.Entities.DTOs;
using Core.Domain.Entities.SDK.Estructuras;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Domain.SDK_Comercial;
using System.Collections.ObjectModel;

namespace Core.Application.ViewModels
{
    public class VMCreateDocumento : ViewModelBase
    {
        private readonly IDocumentoService _documentoService = null!;
        private readonly TerminalSettings _terminalSettings = null!;
        private DocumentoDto _documento = null!;
        private ClienteProveedorDto _clienteProveedorSeleccionado = null!;
        public DocumentoDto Documento
        {
            get => _documento;
            set
            {
                _documento = value;
                OnPropertyChanged(nameof(Documento));
            }
        }
        private ObservableCollection<ViewProductoUnidades> _productos = new();
        public ObservableCollection<ViewProductoUnidades> Productos
        {
            get => _productos;
            set
            {
                _productos = value;
                OnPropertyChanged(nameof(Productos));
            }
        }

        public ClienteProveedorDto ClienteProveedorSeleccionado
        {
            get => _clienteProveedorSeleccionado;
            set
            {
                _clienteProveedorSeleccionado = value;
                OnPropertyChanged(nameof(ClienteProveedorSeleccionado));
            }
        }

        public VMCreateDocumento(IDocumentoService documentoService, TerminalSettings terminalSettings)
        {
            _documentoService = documentoService;
            _terminalSettings = terminalSettings;

            _documento = new();
            _documento.CodConcepto = terminalSettings.CodigoConcepto;
            _documento.Serie = terminalSettings.Serie;
            _documento.CodigoCteProv = terminalSettings.CodigoCteProv;
            _documento.Referencia = terminalSettings.Referencia;

            _documento.Fecha = DateTime.Now.ToString("MM/dd/yyyy");
            _clienteProveedorSeleccionado = new ClienteProveedorDto();
            _clienteProveedorSeleccionado.CRAZONSOCIAL = "Seleccionar Socio";
        }

        public VMCreateDocumento() { }

        /// <summary>
        /// Deberias hacer un pop despues de usar este metodo, claro, si sae bien
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task EnviarDocumentoMovimientos()
        {
            if (_clienteProveedorSeleccionado.CCODIGOCLIENTE == null)
            {
                throw new Exception("Debe seleccionar un cliente valido");
            }

            //validamos y llenamos los movimientos
            var movimientos = new List<MovimientoDto>();
            foreach (var producto in Productos)
            {
                if (producto.Unidades == 0)
                {
                    throw new Exception($"El Producto {producto.Producto.CNOMBREPRODUCTO} tiene 0 unidades, porfavor captura las que falten");
                }
                MovimientoDto newMovimiento = new();
                newMovimiento.CodigoProducto = producto.Producto.CCODIGOPRODUCTO;
                newMovimiento.CodigoAlmacen = _terminalSettings.CodigoAlmacen;
                newMovimiento.Unidades = producto.Unidades;
                newMovimiento.Referencia = Documento.Referencia;

                movimientos.Add(newMovimiento);
            }

            Documento.RazonSocial = ClienteProveedorSeleccionado.CRAZONSOCIAL;

            Documento = await _documentoService.PostPendientes(_documento, movimientos);

            OnPropertyChanged(nameof(Documento));
        }
    }
}
