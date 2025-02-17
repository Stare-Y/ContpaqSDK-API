using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Repositories.SQL;
using Core.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UseCases.SQL.Productos
{
    public class GetProductosByCodigosSQLUseCase
    {
        private readonly IProductoSQLRepo _productRepo;
        private readonly ILogger _logger;

        public GetProductosByCodigosSQLUseCase(IProductoSQLRepo productRepo, ILogger logger)
        {
            _productRepo = productRepo;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene un producto por su codigo
        /// </summary>
        /// <param name="codigoProducto"></param>
        public async Task<IEnumerable<ProductoDto>> Execute(IEnumerable<string> codigos)
        {
            _logger.Log($"Obteniendo solicitud de buscar productos por lista de codigos, codigos: {codigos.Count()}");

            List<ProductoDto> productos = new List<ProductoDto>();

            foreach (var codigo in codigos)
            {
                var producto = await _productRepo.GetByCodigoAsync(codigo);
                productos.Add(new ProductoDto(producto));
            }

            if (productos.Count == 0)
                throw new KeyNotFoundException("No se encontraron productos con los codigos proporcionados");

            return productos;
        }
    }
}
