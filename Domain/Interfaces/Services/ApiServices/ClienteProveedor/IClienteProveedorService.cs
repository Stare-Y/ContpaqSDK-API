using Core.Domain.Entities.DTOs;

namespace Core.Domain.Interfaces.Services.ApiServices.ClienteProveedor
{
    public interface IClienteProveedorService
    {
        /// <summary>
        /// Obtiene los clientes/proveedores por su nombre
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ClienteProveedorDto>> SearchAsync(string name);
    }
}
