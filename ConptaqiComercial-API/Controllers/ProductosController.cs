using Core.Application.UseCases.SQL.Productos;
using Core.Domain.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Pedidos_CPE.Controllers
{
    [ApiController]
    public class ProductosController : Controller
    {
        private readonly SearchProductosByNameSQLUseCase _searchProductosByNameSQLUseCase;
        //private readonly GetProductosByIdsSQLUseCase _getProductosByIdsSQLUseCase;
        private readonly GetProductosByCodigosSQLUseCase _getProductoByCodigosSQL;
        public ProductosController(SearchProductosByNameSQLUseCase searchProductosByNameSQLUseCase, GetProductosByIdsSQLUseCase getProductosByIdsSQLUseCase,
            GetProductosByCodigosSQLUseCase getProductoByCodigoSQLUseCase, GetProductosByCodigosSQLUseCase getProductosByCodigosSQLUseCase)
        {
            _searchProductosByNameSQLUseCase = searchProductosByNameSQLUseCase;
            _getProductoByCodigosSQL = getProductoByCodigoSQLUseCase;
            //_getProductosByIdsSQLUseCase = getProductosByIdsSQLUseCase;
            //_getProductoByCodigoSQL = getProductoByCodigoSQLUseCase;
        }

        [HttpGet]
        [Route("Productos/ByNombre")]
        public async Task<ActionResult<ApiResponse>> GetProductosByName(string nombre)
        {
            try
            {
                var productos = await _searchProductosByNameSQLUseCase.Execute(nombre);
                return Ok(new ApiResponse { Message = $"Se encontraron {productos.Count()} productos con la busqueda: {nombre}", Data = productos, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Message = $"Error obteniendo productos con la busqueda {nombre}", Success = false, ErrorDetails = ex.Message});
            }
        }

        [HttpGet]
        [Route("Productos/ByCodigos")]
        public async Task<ActionResult<ApiResponse>> GetProductosByCodigos([FromQuery] List<string> codigos)
        {
            try
            {
                var productos = await _getProductoByCodigosSQL.Execute(codigos);
                return Ok(new ApiResponse { Message = $"Productos encontrados por codgo: {productos.Count()}", Data = productos, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Message = $"Error obteniendo productos con los codigos proporcionados", Success = false, ErrorDetails = ex.Message });
            }
        }
    }
}
