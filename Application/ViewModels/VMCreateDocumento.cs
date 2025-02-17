using Core.Application.ViewModels.Base;
using Core.Application.ViewModels.ObservableEntity;
using Core.Domain.Entities;
using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using System.Collections.ObjectModel;

namespace Core.Application.ViewModels
{
    public class VMCreateDocumento : ViewModelBase
    {
        private readonly IDocumentoService _documentoService = null!;
        private readonly TerminalSettings _terminalSettings = null!;

        public DocumentoDto Documento
        {
            get;
            set;
        }

        public ObservableCollection<ViewProductoUnidades> Productos
        {
            get;
            set;
        }

        public ClienteProveedorDto ClienteProveedorSeleccionado
        {
            get;
            set;
        }

        public VMCreateDocumento(IDocumentoService documentoService, TerminalSettings terminalSettings)
        {
            _documentoService = documentoService;
            _terminalSettings = terminalSettings ?? throw new ArgumentNullException(nameof(terminalSettings));

            Documento = new();
            

            ClienteProveedorSeleccionado = new ClienteProveedorDto();
            ClienteProveedorSeleccionado.CRAZONSOCIAL = "Seleccionar Socio";
            Productos = new ();
        }

        public VMCreateDocumento()
        {
            Documento = new DocumentoDto();
            ClienteProveedorSeleccionado = new ClienteProveedorDto { CRAZONSOCIAL = "Seleccionar Socio (Default BUilder)" };
            Productos = new();
        }

        /// <summary>
        /// Deberias hacer un pop despues de usar este metodo, claro, si sae bien
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task EnviarDocumentoMovimientos()
        {
            if (ClienteProveedorSeleccionado.CCODIGOCLIENTE == null)
            {
                throw new Exception("Debe seleccionar un cliente valido");
            }

            if (_terminalSettings == null)
            {
                throw new ArgumentNullException();
            }

            Documento.CodConcepto = _terminalSettings.CodigoConcepto;
            Documento.Serie = _terminalSettings.Serie;
            Documento.CodigoCteProv = _terminalSettings.CodigoCteProv;
            Documento.Referencia = _terminalSettings.Referencia;
            Documento.CodConcepto = _terminalSettings.CodigoConcepto;
            Documento.Fecha = DateTime.Now.ToString("MM/dd/yyyy");

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

            Documento = await _documentoService.PostPendientes(Documento, movimientos);

            OnPropertyChanged(nameof(Documento));
        }
    }
}
