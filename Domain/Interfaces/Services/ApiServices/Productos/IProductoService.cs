using Core.Domain.Entities.DTOs;

namespace Core.Domain.Interfaces.Services.ApiServices.Productos
{
    public interface IProductoService
    {
        /// <summary>
        /// Obtiene los productos por una lista de ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns>COLECCION DE PRODUCTOS</returns>
        /// <exception cref="Exception"></exception>
        Task<IEnumerable<ProductoDto>> GetByIdsAsync(IEnumerable<int> ids);

        /// <summary>
        /// obtiene los productos por una lista de codigos
        /// </summary>
        /// <param name="codigos"></param>
        /// <returns>COLECCION DE PRODUCTOS</returns>
        Task<IEnumerable<ProductoDto>> GetByCodigosAsync(IEnumerable<string> codigos);


        /// <summary>
        /// busca los productos con nombre
        /// </summary>
        /// <param name="nombre"></param>
        /// <returns>COLECCION DE PRODUCTOS</returns>
        Task<IEnumerable<ProductoDto>> SearchByNombreAsync(string nombre);
    }
}
