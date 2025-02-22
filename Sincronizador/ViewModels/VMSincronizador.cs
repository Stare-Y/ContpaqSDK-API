using Core.Application.ViewModels.Base;
using Core.Domain.Entities.DTOs;
using Core.Domain.Entities.SQL;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Core.Domain.Interfaces.Services.ApiServices.SDK;
using Infrastructure.Context;
using Infrastructure.Services.API.SDK;
using Microsoft.EntityFrameworkCore;
using Sincronizador.Models;
using System.Collections.ObjectModel;

namespace Sincronizador.ViewModels
{
    public class VMSincronizador : ViewModelBase
    {
        private readonly ISDKService _sdkService;
        private readonly DbContextOptions<ContpaqiSQLContext> _primaryDbOptions;
        private readonly DbContextOptions<ContpaqiSQLContext> _secondaryDbOptions;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int Progreso { get; set; } = 0;

        private string _primaryEmpresaName;
        public string PrimaryEmpresaName
        {
            get => _primaryEmpresaName + $", {PrimaryDocumentos.Count} encontrados";
            set => _primaryEmpresaName = value;
        }
        private string _secondaryEmpresaName;
        public string SecondaryEmpresaName
        {
            get => _secondaryEmpresaName + $", {SecondaryDocumentos.Count} encontrados";
            set => _secondaryEmpresaName = value;
        }
        public string FaltantesCount => $"Diferencia de documentos: {FaltantesEnSecondary.Count}";
        public string Concepto { get; set; }

        private string _targetEmpresa;
        private string _codigoAlmacen;
        private string _codigoClasificacion;

        public ObservableCollection<DocumentoSQL> PrimaryDocumentos { get; set; } = new();

        public ObservableCollection<DocumentoSQL> SecondaryDocumentos { get; set; } = new();
        public ObservableCollection<DocumentoSQL> FaltantesEnSecondary { get; set; } = new();
        public List<DocumentoSQL> DocumentosSeleccionados { get; set; } = new();

        public VMSincronizador(DbContextOptions<ContpaqiSQLContext> primaryDbOptions,
            DbContextOptions<ContpaqiSQLContext> secondaryDbOptions, SincronizadorSettings sincronizadorSettings, 
            ISDKService sdkService)
        {
            _primaryDbOptions = primaryDbOptions;
            _secondaryDbOptions = secondaryDbOptions;
            Concepto = sincronizadorSettings.ConceptoDefault ?? throw new Exception("ConceptoDefault fue nulo");
            _sdkService = sdkService;
            FechaFin = DateTime.Today;
            FechaInicio = DateTime.Today.AddDays(-30);

            _primaryEmpresaName = sincronizadorSettings.PrimaryEmpresaName ?? "Empresa Primaria";
            _secondaryEmpresaName = sincronizadorSettings.SecondaryEmpresaName ?? "Empresa Secundaria";

            _targetEmpresa = sincronizadorSettings.TargetEmpresa ?? throw new Exception("TargetEmpresa fue nulo");
            _codigoAlmacen = sincronizadorSettings.CodigoAlmacen ?? throw new Exception("CodigoAlmacen fue nulo");
            _codigoClasificacion = sincronizadorSettings.CodigoClasificacion ?? throw new Exception("CodigoClasificacion fue nulo");
        }

        #region GetDocumentosFiltrados

        public async Task GetDocumentosFiltrados()
        {
            ValidarParametrosConsulta();

            await ActualizarListasDocumentos();
        }

        private void ValidarParametrosConsulta()
        {
            if (FechaFin < FechaInicio)
            {
                throw new Exception("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            if (FechaFin == default || FechaInicio == default)
            {
                throw new Exception("Las fechas no pueden ser nulas");
            }

            if (string.IsNullOrEmpty(Concepto))
            {
                throw new Exception("El concepto no puede ser nulo o vacio");
            }
        }

        private async Task ActualizarListasDocumentos()
        {
            ConceptoSQL concepto = await GetConcepto();

            await GetPrimaryDocumentos(concepto);

            await GetSecondaryDocumentos(concepto);

            SepararFaltantes();

            NotificarDocumentosActualizados();
        }

        private async Task<ConceptoSQL> GetConcepto()
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.Conceptos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CCODIGOCONCEPTO == Concepto) ??
                        throw new KeyNotFoundException("Error, el concepto proporcionado no se encontro en la base de datos.");
            }
        }
        private async Task GetPrimaryDocumentos(ConceptoSQL concepto)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                var documentos = await primarySQLDbContext.Documentos
                    .AsNoTracking()
                    .Where(d => d.CFECHA >= FechaInicio && d.CFECHA <= FechaFin && concepto.CIDCONCEPTODOCUMENTO == d.CIDCONCEPTODOCUMENTO)
                    .ToListAsync();

                PrimaryDocumentos.Clear();
                foreach (var doc in documentos)
                {
                    PrimaryDocumentos.Add(doc);
                }
            }
        }

        private async Task GetSecondaryDocumentos(ConceptoSQL concepto)
        {
            using (var secondarySQLDbContext = new ContpaqiSQLContext(_secondaryDbOptions))
            {
                var documentos = await secondarySQLDbContext.Documentos
                    .AsNoTracking()
                    .Where(d => d.CFECHA >= FechaInicio && d.CFECHA <= FechaFin && concepto.CIDCONCEPTODOCUMENTO == d.CIDCONCEPTODOCUMENTO)
                    .ToListAsync();

                SecondaryDocumentos.Clear();
                foreach (var doc in documentos)
                {
                    SecondaryDocumentos.Add(doc);
                }
            }
        }
        private void SepararFaltantes()
        {
            FaltantesEnSecondary.Clear();
            foreach (var documento in PrimaryDocumentos)
            {
                if (!SecondaryDocumentos.Any(d => d.CFOLIO == documento.CFOLIO && d.CSERIEDOCUMENTO == documento.CSERIEDOCUMENTO))
                {
                    FaltantesEnSecondary.Add(documento);
                }
            }
        }

        public void NotificarDocumentosActualizados()
        {
            OnCollectionChanged(nameof(PrimaryDocumentos));
            OnCollectionChanged(nameof(SecondaryDocumentos));
            OnCollectionChanged(nameof(FaltantesEnSecondary));
            OnPropertyChanged(nameof(FaltantesCount));
            OnPropertyChanged(nameof(PrimaryEmpresaName));
            OnPropertyChanged(nameof(SecondaryEmpresaName));
        }

        #endregion

        #region PostToSDK

        public async Task PostDocumentoToSDK(DocumentoSQL documentoSQL)
        {
            DocumentoDto documentoDto = await BuildDocumentoDto(documentoSQL);

            List<MovimientoDto> movimientoDtos = new(await GetMovimientoDtos(documentoSQL.CIDDOCUMENTO));

            await _sdkService.PostDocumentoSDK(documentoDto, movimientoDtos, _targetEmpresa);

            await ActualizarListasDocumentos();
        }

        #region BuildDocumentoDto

        private async Task<DocumentoDto> BuildDocumentoDto(DocumentoSQL documentoSQL)
        {
            ConceptoSQL concepto = await GetConcepto();

            ClienteProveedorSQL clienteProveedorPrimary = await GetClienteProveedorPrimary(documentoSQL);

            ClienteProveedorSQL clienteProveedorSecondary = await GetClienteProveedorSecondary(clienteProveedorPrimary);

            DocumentoDto documentoDto = new();

            documentoDto.Folio = documentoSQL.CFOLIO;
            documentoDto.DescuentoDoc1 = documentoSQL.CDESCUENTODOC1;
            documentoDto.DescuentoDoc2 = documentoSQL.CDESCUENTODOC2;
            documentoDto.SistemaOrigen = documentoSQL.CSISTORIG;
            documentoDto.CodConcepto = concepto.CCODIGOCONCEPTO;
            documentoDto.Serie = documentoSQL.CSERIEDOCUMENTO;
            documentoDto.Fecha = documentoSQL.CFECHA.ToString("MM/dd/yyyy");

            //buscar el codigo del cliente/proveedor en la base secundaria
            documentoDto.CodigoCteProv = clienteProveedorSecondary.CCODIGOCLIENTE;
            documentoDto.Referencia = documentoSQL.CREFERENCIA;
            documentoDto.Gasto1 = documentoSQL.CGASTO1;
            documentoDto.Gasto2 = documentoSQL.CGASTO2;
            documentoDto.Gasto3 = documentoSQL.CGASTO3;
            documentoDto.Observaciones = documentoSQL.COBSERVACIONES ?? string.Empty;
            documentoDto.TextoExtra1 = documentoSQL.CTEXTOEXTRA1 ?? string.Empty;
            documentoDto.TextoExtra2 = documentoSQL.CTEXTOEXTRA2 ?? string.Empty;
            documentoDto.TextoExtra3 = documentoSQL.CTEXTOEXTRA3 ?? string.Empty;

            return documentoDto;
        }
        
        private async Task<ClienteProveedorSQL> GetClienteProveedorPrimary(DocumentoSQL documentoSQL)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.ClientesProveedores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cp => cp.CIDCLIENTEPROVEEDOR == documentoSQL.CIDCLIENTEPROVEEDOR) ??
                        throw new KeyNotFoundException($"El documento {documentoSQL.CSERIEDOCUMENTO} {documentoSQL.CFOLIO} pertenece a un cliente proveedor invalido, parece estar corrupto");
            }
        }
        private async Task<ClienteProveedorSQL> GetClienteProveedorSecondary(ClienteProveedorSQL clienteProveedorPrimary)
        {
            using (var secondarySQLDbContext = new ContpaqiSQLContext(_secondaryDbOptions))
            {
                return await secondarySQLDbContext.ClientesProveedores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cp => cp.CCODIGOCLIENTE == clienteProveedorPrimary.CCODIGOCLIENTE) ??
                        throw new KeyNotFoundException($"El cliente/proveedor {clienteProveedorPrimary.CRAZONSOCIAL} con codigo {clienteProveedorPrimary.CCODIGOCLIENTE} debe existir tambien en la empresa {_secondaryEmpresaName}.");
            }
        }

        #endregion

        #region BuildMovimientosDtos

        private async Task<IEnumerable<MovimientoDto>> GetMovimientoDtos(int idDocumento)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                var movimientosPrimary = await primarySQLDbContext.Movimientos
                    .AsNoTracking()
                    .Where(m => m.CIDDOCUMENTO == idDocumento)
                    .ToListAsync();

                //search movimientos in secondary db
                var movimientosSecondary = new List<MovimientoDto>();

                foreach (var movimientoPrimary in movimientosPrimary)
                {
                    ProductoSQL productoPrimary = await GetProductoPrimary(movimientoPrimary);
                    ProductoSQL productoSecondary = await GetProductoSecondary(productoPrimary);

                    MovimientoDto movimientoDto = new()
                    {
                        CodigoProducto = productoSecondary.CCODIGOPRODUCTO,
                        CodigoAlmacen = await GetCodigoAlmacen(movimientoPrimary.CIDALMACEN),
                        CodigoClasificacion = await GetCodigoClasificacion(movimientoPrimary.CIDVALORCLASIFICACION),
                        Unidades = movimientoPrimary.CUNIDADES,
                        Precio = movimientoPrimary.CPRECIOCAPTURADO,
                        Costo = movimientoPrimary.CCOSTOCAPTURADO,
                        Fecha = movimientoPrimary.CFECHA,
                        Referencia = movimientoPrimary.CREFERENCIA
                    };

                    movimientosSecondary.Add(movimientoDto);
                }

                return movimientosSecondary;
            }
        }

        private async Task<ProductoSQL> GetProductoPrimary(MovimientoSQL movimientoSQl)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CIDPRODUCTO == movimientoSQl.CIDPRODUCTO) ??
                        throw new KeyNotFoundException($"Un Movimiento contenia un producto invalido, parece que el documento primario esta corrupto(idDocumento: {movimientoSQl.CIDDOCUMENTO})");
            }
        }

        private async Task<ProductoSQL> GetProductoSecondary(ProductoSQL productoPrimary)
        {
            using (var secondarySQLDbContext = new ContpaqiSQLContext(_secondaryDbOptions))
            {
                return await secondarySQLDbContext.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CCODIGOPRODUCTO == productoPrimary.CCODIGOPRODUCTO) ??
                        throw new KeyNotFoundException($"El producto {productoPrimary.CNOMBREPRODUCTO} con codigo {productoPrimary.CCODIGOPRODUCTO} debe existir tambien en la empresa {_secondaryEmpresaName}.");
            }
        }
        private async Task<string> GetCodigoAlmacen(int idAlmacen)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.Database
                    .SqlQuery<string>($"SELECT TOP 1 CCODIGOALMACEN AS Value FROM admAlmacenes WHERE CIDALMACEN = {idAlmacen}")
                    .SingleOrDefaultAsync() ?? "1";
            }
        }

        private async Task<string> GetCodigoClasificacion(int idCodigoClasificacion)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.Database
                    .SqlQuery<string>($"SELECT TOP 1 CCODIGOVALORCLASIFICACION AS Value FROM admClasificacionesValores WHERE CIDVALORCLASIFICACION = {idCodigoClasificacion}")
                    .SingleOrDefaultAsync() ?? "0";
            }
        }

        #endregion

        #endregion
    }
}
