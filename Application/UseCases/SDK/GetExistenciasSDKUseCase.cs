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

        public async Task<double> Execute(string codigoProducto, string codigoAlmacen, DateTime fecha)
        {
            _logger.Log("Ejecutando caso de uso GetExistenciasSDKUseCase...");
            while (true)
            {
                var canWork = await _sdkRepo.StartTransaction("test");
                if (canWork)
                {
                    double existencias = await _sdkRepo.GetExistencias(codigoProducto,codigoAlmacen, fecha);

                    _sdkRepo.StopTransaction();

                    _logger.Log($"Se encontraron {existencias} para el producto {codigoProducto} en el almacen {codigoAlmacen}");

                    return existencias;
                }
                else
                {
                    _logger.Log("SDK Ocupado, esperando turno para GetExistenciasSDKUseCase...");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
