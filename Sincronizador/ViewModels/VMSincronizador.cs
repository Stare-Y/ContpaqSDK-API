using Core.Application.ViewModels.Base;
using Core.Domain.Entities.SQL;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace Sincronizador.ViewModels
{
    public class VMSincronizador : ViewModelBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly DbContextOptions<ContpaqiSQLContext> _primaryDbOptions;
        private readonly DbContextOptions<ContpaqiSQLContext> _secondaryDbOptions;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int Progreso { get; set; } = 0;

        public string Concepto { get; set; }

        public ObservableCollection<DocumentoSQL> PrimaryDocumentos { get; set; } = new();

        public ObservableCollection<DocumentoSQL> SecondaryDocumentos { get; set; } = new();
        public ObservableCollection<DocumentoSQL> FaltantesEnSecondary { get; set; } = new();

        public VMSincronizador(DbContextOptions<ContpaqiSQLContext> primaryDbOptions, 
            DbContextOptions<ContpaqiSQLContext> secondaryDbOptions, string concepto, 
            IDocumentoService documentoService)
        {
            _primaryDbOptions = primaryDbOptions;
            _secondaryDbOptions = secondaryDbOptions;
            Concepto = concepto;
            _documentoService = documentoService;
            FechaFin = DateTime.Today;
            FechaInicio = DateTime.Today.AddDays(-30);
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

        private void NotificarDocumentosActualizados()
        {
            OnPropertyChanged(nameof(PrimaryDocumentos));
            OnPropertyChanged(nameof(SecondaryDocumentos));
            OnPropertyChanged(nameof(FaltantesEnSecondary));
        }

        public Task PostDocumentoToSDK(DocumentoSQL documento)
        {
            throw new NotImplementedException();
            //////////////all into a foreach lol
            ////Get movimientos from SQL to prepare the Dto Elements

            ////Build request
            //DocumentoDto documentoDto = new();
            //List<MovimientoDto> movimientoDtos = new();

            ////Post
            //await _documentoService.PostDocumentoSDK(documentoDto, movimientoDtos);
        }
    }
}
