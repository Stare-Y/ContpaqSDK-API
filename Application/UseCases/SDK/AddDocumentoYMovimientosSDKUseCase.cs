using Core.Domain.Entities.DTOs;
using Core.Domain.Exceptions;
using Core.Domain.Interfaces.Repositories;
using Core.Domain.Interfaces.Services;
using System.Diagnostics;
namespace Core.Application.UseCases.SDK
{
    public class AddDocumentoYMovimientosSDKUseCase
    {
        private readonly ISDKRepo _sdkRepo;

        public AddDocumentoYMovimientosSDKUseCase(ISDKRepo sDKRepo, ILogger logger)
        {
            _sdkRepo = sDKRepo;
        }

        public async Task<Dictionary<int, double>> Execute(DocumentoDto documentoDto, IEnumerable<MovimientoDto> movimientoDtos, string empresa)
        {
            Trace.WriteLine("Ejecutando caso de uso AddDocumentoYMovimientosSDK...");

            await _sdkRepo.StartTransaction(empresa);
            
            Dictionary<int, double> addDocumentResult = new();
            try
            {
                addDocumentResult = await _sdkRepo.AddDocumento(documentoDto);
                int idDocumento = addDocumentResult.Keys.First();

                foreach (var movimiento in movimientoDtos)
                {
                    await _sdkRepo.AddMovimiento(movimiento, idDocumento);
                }

                _sdkRepo.StopTransaction();

                Trace.WriteLine($"Se genero un nuevo documento para la empresa {empresa}. Id SQL: {idDocumento}, Serie: {documentoDto.Serie}, Folio: {addDocumentResult[idDocumento]}. ");

                return addDocumentResult;
            }
            finally
            {
                _sdkRepo.StopTransaction();
            }
        }
    }
}
