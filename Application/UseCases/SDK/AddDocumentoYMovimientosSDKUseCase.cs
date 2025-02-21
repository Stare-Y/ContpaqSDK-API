using Core.Domain.Entities.DTOs;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces.Repositories;
using Core.Domain.Interfaces.Services;
namespace Core.Application.UseCases.SDK
{
    public class AddDocumentoYMovimientosSDKUseCase
    {
        private readonly ISDKRepo _sdkRepo;
        private readonly ILogger _logger;

        public AddDocumentoYMovimientosSDKUseCase(ISDKRepo sDKRepo, ILogger logger)
        {
            _logger = logger;
            _sdkRepo = sDKRepo;
        }

        public async Task<Dictionary<int, double>> Execute(DocumentoDto documentoDto, IEnumerable<MovimientoDto> movimientoDtos, string empresa)
        {
            _logger.Log("Ejecutando caso de uso AddDocumentoYMovimientosSDK...");

            while (true)
            {
                var canWork = await _sdkRepo.StartTransaction(empresa);
                if (canWork)
                {
                    Dictionary<int, double> addDocumentResult = new();
                    try
                    {
                        addDocumentResult = await _sdkRepo.AddDocumento(documentoDto);
                    }
                    catch(SDKException ex)
                    {
                        _sdkRepo.StopTransaction();
                        throw new Exception(ex.Message);
                    }

                    int idDocumento = addDocumentResult.Keys.First();

                    foreach (var movimiento in movimientoDtos)
                    {
                        await _sdkRepo.AddMovimiento(movimiento, idDocumento);
                    }

                    _sdkRepo.StopTransaction();

                    _logger.Log($"Se genero un nuevo documento para la empresa {empresa}. Id SQL: {idDocumento}, Serie: {documentoDto.Serie}, Folio: {addDocumentResult[idDocumento]}. ");

                    return addDocumentResult;
                }
                else
                {
                    _logger.Log("SDK Ocupado, esperando turno para AddDocumentoYMovimientosSDK...");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
