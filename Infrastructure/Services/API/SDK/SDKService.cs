using Core.Application.UseCases.SDK.Requests;
using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.SDK;
using Newtonsoft.Json;

namespace Infrastructure.Services.API.SDK
{
    public class SDKService : ISDKService
    {
        private readonly IApiService _apiService;
        public SDKService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<double> GetExistenciasAsync(string codigoProducto, string codigoAlmacen, DateTime fecha)
        {
            var response = await _apiService.GetAsync<ApiResponse>($"SDK/GetExistencias?codigoProducto={codigoProducto}&codigoAlmacen={codigoAlmacen}&fecha={fecha}");

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception("Error al buscar existencias: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var existencias = JsonConvert.DeserializeObject<double>(json);

            return existencias;
        }

        public async Task<Dictionary<int, double>> PostDocumentoSDK(DocumentoDto document, IEnumerable<MovimientoDto> movements, string empresa)
        {
            var request = new AddDocumentoYMovimientosSDKRequest
            {
                DocumentoDto = document,
                MovimientoDtos = [.. movements],
                Empresa = empresa
            };

            var response = await _apiService.PostAsync<ApiResponse>("SDK/addDocumentoYMovimientos", request);

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception("Error al crear documento en SDK: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var documentoCreado = JsonConvert.DeserializeObject<Dictionary<int, double>>(json);

            return documentoCreado ?? throw new Exception("Error al crear documento en SDK, la instancia de respuesta fue nula.");
        }

        public async Task<bool> IsSDKGood()
        {
            var response = await _apiService.GetAsync<ApiResponse>("SDK/isGood");

            return response.Success;
        }
    }
}
