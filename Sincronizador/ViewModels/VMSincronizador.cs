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

        public async Task GetDocumentosFiltrados()
        {
            ValidarParametrosConsulta();

            await ActualizarListasDocumentos();
        }

        private async Task ActualizarListasDocumentos()
        {
            ConceptoSQL concepto = await GetConcepto();

            await GetPrimaryDocumentos(concepto);

            await GetSecondaryDocumentos(concepto);

            SepararFaltantes();

            NotificarDocumentosActualizados();
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
        private async Task<ClienteProveedorSQL> GetClienteProveedor(int idClienteProveedor)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.ClientesProveedores
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cp => cp.CIDCLIENTEPROVEEDOR == idClienteProveedor) ??
                        throw new KeyNotFoundException("Error, el cliente/proveedor proporcionado no se encontro en la base de datos.");
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

        public async Task PostDocumentoToSDK(DocumentoSQL documentoSQL)
        {
            DocumentoDto documentoDto = await BuildDocumentoDto(documentoSQL);

            List<MovimientoDto> movimientoDtos = new (await GetMovimientoDtos(documentoSQL.CIDDOCUMENTO));

            await _sdkService.PostDocumentoSDK(documentoDto, movimientoDtos, _targetEmpresa);

            await ActualizarListasDocumentos();
        }

        private async Task<DocumentoDto> BuildDocumentoDto(DocumentoSQL documentoSQL)
        {
            var concepto = await GetConcepto();
            var clienteProveedor = await GetClienteProveedor(documentoSQL.CIDCLIENTEPROVEEDOR);

            DocumentoDto documentoDto = new();

            documentoDto.Folio = documentoSQL.CFOLIO;
            documentoDto.DescuentoDoc1 = documentoSQL.CDESCUENTODOC1;
            documentoDto.DescuentoDoc2 = documentoSQL.CDESCUENTODOC2;
            documentoDto.SistemaOrigen = documentoSQL.CSISTORIG;
            documentoDto.CodConcepto = concepto.CCODIGOCONCEPTO;
            documentoDto.Serie = documentoSQL.CSERIEDOCUMENTO;
            documentoDto.Fecha = documentoSQL.CFECHA.ToString("MM/dd/yyyy");
            documentoDto.CodigoCteProv = clienteProveedor.CCODIGOCLIENTE;
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

        private async Task<ProductoSQL> GetProducto(int idProducto)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.Productos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CIDPRODUCTO == idProducto) ??
                        throw new KeyNotFoundException("Error, el producto proporcionado no se encontro en la base de datos.");
            }
        }

        private async Task<IEnumerable<MovimientoDto>> GetMovimientoDtos(int idDocumento)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                var movimientos = await primarySQLDbContext.Movimientos
                    .AsNoTracking()
                    .Where(m => m.CIDDOCUMENTO == idDocumento)
                    .ToListAsync();

                List<MovimientoDto> movimientoDtos = new();

                foreach (var movimientoSQL in movimientos)
                {
                    MovimientoDto movimientoDto = new();

                    movimientoDto.CodigoProducto = (await GetProducto(movimientoSQL.CIDPRODUCTO)).CCODIGOPRODUCTO;
                    movimientoDto.CodigoAlmacen = await GetCodigoAlmacen(movimientoSQL.CIDALMACEN);
                    movimientoDto.Unidades = movimientoSQL.CUNIDADES;
                    movimientoDto.Precio = movimientoSQL.CPRECIO;
                    movimientoDto.Costo = movimientoSQL.CCOSTOCAPTURADO;
                    movimientoDto.Fecha = movimientoSQL.CFECHA;
                    movimientoDto.Referencia = movimientoSQL.CREFERENCIA;
                    movimientoDto.CodigoClasificacion = await GetCodigoClasificacion(movimientoSQL.CIDVALORCLASIFICACION);

                    movimientoDtos.Add(movimientoDto);
                }

                return movimientoDtos;
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

        private async Task<string> GetCodigoAlmacen(int idAlmacen)
        {
            using (var primarySQLDbContext = new ContpaqiSQLContext(_primaryDbOptions))
            {
                return await primarySQLDbContext.Database
                    .SqlQuery<string>($"SELECT TOP 1 CCODIGOALMACEN AS Value FROM admAlmacenes WHERE CIDALMACEN = {idAlmacen}")
                    .SingleOrDefaultAsync() ?? "1";
            }
        }
    }
}
