using Core.Application.UseCases.Postgres.Movimientos;
using Core.Domain.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Pedidos_CPE.Controllers
{
    [ApiController]
    public class MovimientosController : Controller
    {
        private readonly UpdateMovimientosPostgresUseCase _updateMovimientos;
        private readonly GetMovimientosByDocumentoIdPostgresUseCase _getMovimientosByDocumentoIdPostgresUseCase;
        private readonly Core.Domain.Interfaces.Services.ILogger _logger;

        public MovimientosController(UpdateMovimientosPostgresUseCase updateMovimientos, GetMovimientosByDocumentoIdPostgresUseCase getMovimientosByDocumentoIdPostgresUseCase, Core.Domain.Interfaces.Services.ILogger logger)
        {
            _updateMovimientos = updateMovimientos;
            _getMovimientosByDocumentoIdPostgresUseCase = getMovimientosByDocumentoIdPostgresUseCase;
            _logger = logger;
        }

        [HttpPatch]
        [Route("Movimientos")]
        public async Task<ActionResult<ApiResponse>> PatchMovimientosUnidades([FromBody]List<MovimientoDto> movimientos)
        {
            try
            {
                await _updateMovimientos.Execute(movimientos);
                return Ok(new ApiResponse { Message = "Movimientos agregados con éxito", Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Message = $"Error actualizando los movimientos. ", Success = false, ErrorDetails = ex.Message });
            }
        }

        [HttpGet]
        [Route("Movimientos/ByDocumentoId/")]
        public async Task<ActionResult<ApiResponse>> GetMovimientosByDocumentoId(int documentoId)
        {
            try
            {
                var movimientos = await _getMovimientosByDocumentoIdPostgresUseCase.Execute(documentoId);
                return Ok(new ApiResponse { Message = "Movimientos encontrados", Data = movimientos, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Log($"Error obteniendo los movimientos del documento {documentoId}: " + ex.Message + ex.StackTrace);
                return BadRequest(new ApiResponse { Message = $"Error obteniendo los movimientos del documento {documentoId}. ", Success = false, ErrorDetails = ex.Message });
            }
        }
    }
}
