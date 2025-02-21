using Core.Application.UseCases.Postgres;
using Core.Application.UseCases.Postgres.Requests;
using Core.Application.UseCases.SDK;
using Core.Application.UseCases.SDK.Requests;
using Core.Application.UseCases.SQL.Documentos;
using Core.Domain.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace Pedidos_CPE.Controllers
{
    [ApiController]
    public class DocumentosController : Controller
    {
        private readonly Core.Domain.Interfaces.Services.ILogger _logger;

        


        private readonly AddDocumentoYMovimientosDtoUseCase _addDocumentoYMovimientosDto;
        private readonly GetDocumentosPendientesDtoUseCase _getDocumentosPendientesDto;
        private readonly UpdateDocumentoPendienteDtoUseCase _updateDocumentoPendiente;
        private readonly GetDocumentosByIdClienteAndDateSQLUseCase _getDocumentosByIdClienteAndDateSQL;
        public DocumentosController(
            Core.Domain.Interfaces.Services.ILogger logger, AddDocumentoYMovimientosSDKUseCase addDocumentoYMovimientosSDKUseCase,
            TestSDKUseCase testSDKUseCase, SetDocumentoImpresoSDKUseCase setDocumentoImpresoSDKUseCase,
            AddDocumentoYMovimientosDtoUseCase addDocumentoYMovimientosDtoUseCase, GetDocumentosPendientesDtoUseCase getDocumentosPendientesDtoUseCase,
            UpdateDocumentoPendienteDtoUseCase updateDocumentoPendienteDtoUseCase, GetDocumentosByIdClienteAndDateSQLUseCase getDocumentosByIdClienteAndDateSQLUseCase)
        {
            _logger = logger;
            _addDocumentoYMovimientosDto = addDocumentoYMovimientosDtoUseCase;
            _getDocumentosPendientesDto = getDocumentosPendientesDtoUseCase;
            _updateDocumentoPendiente = updateDocumentoPendienteDtoUseCase;
            _getDocumentosByIdClienteAndDateSQL = getDocumentosByIdClienteAndDateSQLUseCase;
        }

        [HttpPost]
        [Route("/Pendientes")]
        public async Task <ActionResult<ApiResponse>> AddDocumentoYMovimientosPendientes(AddDocumentoYMovimientosDtoRequest request)
        {
            try
            {
                var restultDocument = await _addDocumentoYMovimientosDto.Execute(request.Documento, request.Movimientos);

                return Ok(new ApiResponse { Message = $"Documento y movimientos agregados con éxito, Id generado: {restultDocument.IdPostgres}", Data = restultDocument, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al agregar documento y movimientos a Postgres: {ex.Message} (Inner: {ex.InnerException})");
                return BadRequest(new ApiResponse { Message = "Hubo un problema al agregar el documento y/o los movimientos ", Success = false, ErrorDetails = ex.Message});
            }
        }

        [HttpGet]
        [Route("/Pendientes")]
        public async Task<ActionResult<ApiResponse>> GetDocumentosPendientes()
        {
            try
            {
                List<DocumentoDto> documentos = new(await _getDocumentosPendientesDto.Execute());
                return Ok(new ApiResponse { Message = $"Se obtuvieron {documentos.Count} documentos pendientes exitosamente", Data = documentos, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al obtener documentos pendientes de Postgres: {ex.Message}");
                return BadRequest(new ApiResponse { Message = "Hubo un problema obteniendo los documentos pendientes ", Success = false, ErrorDetails = ex.Message });
            }
        }

        [HttpPut]
        [Route("/Pendientes")]
        public async Task<ActionResult<ApiResponse>> UpdateDocumentoPendiente([FromBody] DocumentoDto documento)
        {
            try
            {
                await _updateDocumentoPendiente.Execute(documento);
                return Ok(new ApiResponse { Message = "Documento actualizado con éxito", Success = true });
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al actualizar documento pendiente en Postgres: {ex.Message}");
                return BadRequest(new ApiResponse { Message = "Hubo un problema al actualizar el documento pendiente ", Success = false, ErrorDetails = ex.Message });
            }
        }

        [HttpGet]
        [Route("/SQL/ByClienteAndDate/")]
        public async Task<ActionResult<ApiResponse>> GetDocumentosByIdAndDate([FromQuery] int idCliente, [FromQuery] DateTime fechaInicio, [FromQuery] DateTime fechaFin)
        {
            try
            {
                var documentos = await _getDocumentosByIdClienteAndDateSQL.Execute(idCliente, fechaInicio, fechaFin);
                return Ok(new ApiResponse { Message = "Documentos pendientes obtenidos con éxito", Data = documentos, Success = true });
            }
            catch (Exception ex)
            {
                _logger.Log($"Error al obtener documentos pendientes de SQL: {ex.Message}");
                return BadRequest(new ApiResponse { Message = "Hubo un problema al obtener los documentos pendientes ", Success = false, ErrorDetails = ex.Message });
            }
        }
    }
}
