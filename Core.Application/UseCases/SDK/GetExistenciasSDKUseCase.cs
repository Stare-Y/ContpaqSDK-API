using Core.Domain.Interfaces.Repositories;
using Core.Domain.Interfaces.Services;

namespace Core.Application.UseCases.SDK
{
    public class GetExistenciasSDKUseCase
    {
        private readonly ISDKRepo _sdkRepo;

        private readonly ILogger _logger;

        public GetExistenciasSDKUseCase(ISDKRepo sdkRepo, ILogger logger)
        {
            _sdkRepo = sdkRepo;
            _logger = logger;
        }

        public async Task<double> Execute(string codigoProducto, string codigoAlmacen, DateTime fecha, string empresa)
        {
            await _logger.Log("Ejecutando caso de uso GetExistenciasSDKUseCase...");
            try
            {
                 await _sdkRepo.StartTransaction(empresa);
               
                double existencias = await _sdkRepo.GetExistencias(codigoProducto,codigoAlmacen, fecha);

                await _logger.Log($"Se encontraron {existencias} para el producto {codigoProducto} en el almacen {codigoAlmacen}");

                return existencias;
            }
            finally
            {
                _sdkRepo.StopTransaction();
            }
        }
    }
}
