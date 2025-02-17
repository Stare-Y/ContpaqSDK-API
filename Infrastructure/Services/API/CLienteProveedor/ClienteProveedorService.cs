using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.ClienteProveedor;

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
            var response = await _apiService.GetAsync<ApiResponse>($"/ClienteProveedor/ByNombre/{name}");
            //parse response.Data to IEnumerable<ClienteProveedorDto>

            List<ClienteProveedorDto>? clientesProveedores = response.Data as List<ClienteProveedorDto>;

            if (!string.IsNullOrEmpty( response.ErrorDetails) && !response.Success)
                throw new Exception("Error al buscar cliente/proveedor: " + response.ErrorDetails);

            return clientesProveedores ?? throw new Exception("Error al buscar cliente/proveedor, la instancia de respuesta fue nula.");
        }
    }
}
