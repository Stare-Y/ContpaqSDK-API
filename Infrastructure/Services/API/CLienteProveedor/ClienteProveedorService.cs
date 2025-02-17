using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.ClienteProveedor;
using Newtonsoft.Json;

namespace Infrastructure.Services.API.CLienteProveedor
{
    public class ClienteProveedorService : IClienteProveedorService
    {
        private readonly IApiService _apiService;

        public ClienteProveedorService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<ClienteProveedorDto>> SearchAsync(string name)
        {
            var response = await _apiService.GetAsync<ApiResponse>($"/ClienteProveedor/ByNombre?nombre={Uri.EscapeDataString(name)}");
            //parse response.Data to IEnumerable<ClienteProveedorDto>

            if (!string.IsNullOrEmpty( response.ErrorDetails) && !response.Success)
                throw new Exception("Error al buscar cliente/proveedor: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var clientesProveedores = JsonConvert.DeserializeObject<List<ClienteProveedorDto>>(json);

            return clientesProveedores ?? throw new Exception("Error al buscar cliente/proveedor, la instancia de respuesta fue nula.");
        }
    }
}
