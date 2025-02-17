using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.Productos;
using Newtonsoft.Json;

namespace Infrastructure.Services.API.Productos
{
    public class ProductoService : IProductoService
    {
        private readonly IApiService _apiService;

        public ProductoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<ProductoDto>> SearchByNombreAsync(string nombre)
        {
            var response = await _apiService.GetAsync<ApiResponse>($"Productos/ByNombre?nombre={Uri.EscapeDataString(nombre)}");

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception($"Error al buscar productos por nombre {nombre}: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var productos = JsonConvert.DeserializeObject<List<ProductoDto>>(json);

            return productos ?? throw new Exception("Error al buscar productos por nombre, la instancia de respuesta fue nula.");
        }

        public Task<IEnumerable<ProductoDto>> GetByIdsAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProductoDto>> GetByCodigosAsync(IEnumerable<string> codigos)
        {
            var response = await _apiService.GetAsync<ApiResponse>($"Productos/ByCodigos?codigos={string.Join(",", codigos)}");

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception($"Error al buscar productos por codigos {string.Join(",", codigos)}: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var productos = JsonConvert.DeserializeObject<List<ProductoDto>>(json);

            return productos ?? throw new Exception("Error al buscar productos por codigos, la instancia de respuesta fue nula.");
        }
    }
}
