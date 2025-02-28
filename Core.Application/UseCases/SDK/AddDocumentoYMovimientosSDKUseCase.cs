using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Repositories;
using Core.Domain.Interfaces.Services;
using System.Diagnostics;
namespace Core.Application.UseCases.SDK
{
    public class AddDocumentoYMovimientosSDKUseCase
    {
        private readonly ISDKRepo _sdkRepo;
        private readonly ILogger _logger;

        public AddDocumentoYMovimientosSDKUseCase(ISDKRepo sDKRepo, ILogger logger)
        {
            _sdkRepo = sDKRepo;
            _logger = logger;
        }

        public async Task<Dictionary<int, double>> Execute(DocumentoDto documentoDto, IEnumerable<MovimientoDto> movimientoDtos, string empresa)
        {
            await _logger.Log("Ejecutando caso de uso AddDocumentoYMovimientosSDK...");
            
            try
            {
                await _sdkRepo.StartTransaction(empresa);

                Dictionary<int, double> addDocumentResult = new();

                addDocumentResult = await _sdkRepo.AddDocumento(documentoDto);
                int idDocumento = addDocumentResult.Keys.First();

                foreach (var movimiento in movimientoDtos)
                {
                    await _sdkRepo.AddMovimiento(movimiento, idDocumento);
                }

                await _logger.Log($"Se genero un nuevo documento para la empresa {empresa}. Id SQL: {idDocumento}, Serie: {documentoDto.Serie}, Folio: {addDocumentResult[idDocumento]}. ");

                return addDocumentResult;
            }
            finally
            {
                _sdkRepo.StopTransaction();
            }
        }
    }
}
