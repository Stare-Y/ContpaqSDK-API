using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.Productos;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Infrastructure.Services.API.Productos
{
    public class ProductoService : IProductoService
    {
        private readonly IApiService _apiService;

        public ProductoService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<ProductoDto>> SearchByNombreAsync(string nombre)
        {
            var response = await _apiService.GetAsync<ApiResponse>($"Productos/ByNombre/{nombre}");

            List<ProductoDto>? productos = response.Data as List<ProductoDto>;

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception($"Error al buscar productos por nombre {nombre}: " + response.ErrorDetails);

            return productos ?? throw new Exception("Error al buscar productos por nombre, la instancia de respuesta fue nula.");
        }

        public Task<IEnumerable<ProductoDto>> GetByIdsAsync(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ProductoDto>> GetByCodigosAsync(IEnumerable<string> codigos)
        {
            throw new NotImplementedException();
        }
    }
}
