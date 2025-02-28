using Core.Domain.Interfaces.Repositories;
using Core.Domain.Interfaces.Services;

namespace Core.Application.UseCases.SDK
{
    public class SetDocumentoImpresoSDKUseCase
    {
        private readonly ISDKRepo _sdkRepo;
        private readonly ILogger _logger;
        public SetDocumentoImpresoSDKUseCase(ISDKRepo sdkRepo, ILogger logger)
        {
            _sdkRepo = sdkRepo;
            _logger = logger;
        }

        public async Task Execute(int idDocumento, string empresa)
        {
            await _logger.Log("Ejecutando caso de uso SetDocumentoImpresoSDK...");
            try
            {
                await _sdkRepo.StartTransaction(empresa);
                
                await _sdkRepo.SetImpreso(idDocumento, true);

                await _logger.Log($"Documento {idDocumento}, establecido como impreso");

                return;
            }
            finally
            {
                _sdkRepo.StopTransaction();
            }
        }
    }
}
