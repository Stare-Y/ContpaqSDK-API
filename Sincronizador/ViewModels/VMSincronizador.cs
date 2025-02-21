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
        private readonly DbContextOptions<ContpaqiSQLContext> _fiscalOptions;
        private readonly DbContextOptions<ContpaqiSQLContext> _noFiscalOptions;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int Progreso { get; set; } = 0;

        public string Concepto { get; set; }

        public ObservableCollection<DocumentoSQL> DocumentosNoFiscal { get; set; } = new();

        public ObservableCollection<DocumentoSQL> DocumentosFiscal { get; set; } = new();
        public ObservableCollection<DocumentoSQL> FaltantesEnFiscal { get; set; } = new();

        public VMSincronizador(DbContextOptions<ContpaqiSQLContext> optionsFiscal, 
            DbContextOptions<ContpaqiSQLContext> optionsNoFiscal, string concepto, 
            IDocumentoService documentoService)
        {
            _fiscalOptions = optionsFiscal;
            _noFiscalOptions = optionsNoFiscal;
            Concepto = concepto;
            _documentoService = documentoService;
            FechaFin = DateTime.Today;
            FechaInicio = DateTime.Today.AddDays(-30);
        }

        public async Task GetDocumentosFiltrados()
        {
            if(FechaFin < FechaInicio)
            {
                throw new Exception("La fecha de inicio no puede ser mayor a la fecha de fin");
            }

            if(FechaFin == default || FechaInicio == default)
            {
                throw new Exception("Las fechas no pueden ser nulas");
            }

            FaltantesEnFiscal.Clear();

            ConceptoSQL concepto;

            // Obtener documentos No Fiscal
            using (var noFiscalSQLContext = new ContpaqiSQLContext(_noFiscalOptions))
            {
                concepto = noFiscalSQLContext.conceptos.FirstOrDefault(c => c.CCODIGOCONCEPTO == Concepto) ??
                    throw new KeyNotFoundException("Error, el concepto proporcionado no se encontro en la base de datos.") ;

                DocumentosNoFiscal = new ObservableCollection<DocumentoSQL>(
                    await noFiscalSQLContext.documents
                        .Where(d => d.CFECHA >= FechaInicio && d.CFECHA <= FechaFin && concepto.CIDCONCEPTODOCUMENTO == d.CIDCONCEPTODOCUMENTO)
                        .ToListAsync()
                );
            }

            // Obtener documentos Fiscal
            using (var fiscalSQLContext = new ContpaqiSQLContext(_fiscalOptions))
            {
                DocumentosFiscal = new ObservableCollection<DocumentoSQL>(
                    await fiscalSQLContext.documents
                        .Where(d => d.CFECHA >= FechaInicio && d.CFECHA <= FechaFin && concepto.CIDCONCEPTODOCUMENTO == d.CIDCONCEPTODOCUMENTO)
                        .ToListAsync()
                );
            }

            // Separando faltantes en fiscal
            foreach (var documento in DocumentosNoFiscal)
            {
                if (!DocumentosFiscal.Any(d => d.CFOLIO == documento.CFOLIO && d.CSERIEDOCUMENTO == documento.CSERIEDOCUMENTO))
                {
                    FaltantesEnFiscal.Add(documento);
                }
            }

            OnPropertyChanged(nameof(DocumentosNoFiscal));
            OnPropertyChanged(nameof(DocumentosFiscal));
            OnPropertyChanged(nameof(FaltantesEnFiscal));
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
