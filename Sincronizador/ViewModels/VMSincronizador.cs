using Core.Application.ViewModels.Base;
using Core.Domain.Entities.SQL;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Sincronizador.ViewModels
{
    public class VMSincronizador : ViewModelBase
    {
        private readonly IDocumentoService _documentoService;
        private readonly ContpaqiSQLContext _fiscalSQLContext;
        private readonly ContpaqiSQLContext _noFisalSQLContext;

        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public int Progreso { get; set; } = 0;

        public string Serie { get; set; }

        public ObservableCollection<DocumentoSQL> DocumentosNoFiscal { get; set; } = new();

        public ObservableCollection<DocumentoSQL> DocumentosFiscal { get; set; } = new();
        public ObservableCollection<DocumentoSQL> FaltantesEnFiscal { get; set; } = new();

        public VMSincronizador(DbContextOptions<ContpaqiSQLContext> optionsFiscal, 
            DbContextOptions<ContpaqiSQLContext> optionsNoFiscal, string serie, 
            IDocumentoService documentoService)
        {
            _fiscalSQLContext = new ContpaqiSQLContext(optionsFiscal);
            _noFisalSQLContext = new ContpaqiSQLContext(optionsNoFiscal);
            Serie = serie;
            _documentoService = documentoService;
        }
        public async Task GetDocumentosFiltrados()
        {
            FaltantesEnFiscal.Clear();

            DocumentosNoFiscal = new (await _noFisalSQLContext.documents.Where(d => d.CFECHA >= FechaInicio && d.CFECHA <= FechaFin && Serie == d.CSERIEDOCUMENTO).ToListAsync());

            DocumentosFiscal = new (await _fiscalSQLContext.documents.Where(d => d.CFECHA >= FechaInicio && d.CFECHA <= FechaFin && Serie == d.CSERIEDOCUMENTO).ToListAsync());

            //Separando faltantes en fiscal
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
