using Core.Application.UseCases.SDK;
using Core.Application.UseCases.SDK.Requests;
using Core.Domain.Entities.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Pedidos_CPE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SDKController : ControllerBase
    {
        private readonly TestSDKUseCase _testSDK;
        private readonly AddDocumentoYMovimientosSDKUseCase _addDocumentoYMovimientosSDK;
        private readonly SetDocumentoImpresoSDKUseCase _setDocumentoImpresoSDK;
        private readonly GetExistenciasSDKUseCase _getExistenciasSDK;

        private readonly Core.Domain.Interfaces.Services.ILogger _logger;

        public SDKController(
            TestSDKUseCase testSDK, 
            AddDocumentoYMovimientosSDKUseCase addDocumentoYMovimientosSDK, 
            SetDocumentoImpresoSDKUseCase setDocumentoImpresoSDK, 
            Core.Domain.Interfaces.Services.ILogger logger, 
            GetExistenciasSDKUseCase getExistenciasSDK)
        {
            _testSDK = testSDK;
            _addDocumentoYMovimientosSDK = addDocumentoYMovimientosSDK;
            _setDocumentoImpresoSDK = setDocumentoImpresoSDK;
            _logger = logger;
            _getExistenciasSDK = getExistenciasSDK;
        }

        [HttpPost]
        [Route("/SDK/addDocumentoYMovimientos")]
        public async Task<ActionResult<ApiResponse>> AddDocumentoYMovimientosSDK(AddDocumentoYMovimientosSDKRequest request)
        {
            try
            {
                Dictionary<int, double> idFolioDict = await _addDocumentoYMovimientosSDK.Execute(request.DocumentoDto, request.MovimientoDtos, request.Empresa);
                return Ok(new ApiResponse { Message = "Documento y movimientos agregados con éxito ", Data = idFolioDict, Success = true });
            }
            catch (Exception ex)
            {
                await _logger.Log($"Error al agregar documento y movimientos a SDK: {ex.Message}");
                return BadRequest(new ApiResponse { Success = false, Message = "No se pudo agregar el documento, y/o los movimientos al SDK ", ErrorDetails = ex.Message });
            }
        }

        [HttpPut]
        [Route("/SDK/setDocumentoImpreso/")]
        public async Task<ActionResult<ApiResponse>> SetDocumentoImpresoSDK([FromQuery] int idDocumento, [FromQuery] string empresa)
        {
            try
            {
                await _setDocumentoImpresoSDK.Execute(idDocumento, empresa);

                return Ok(new ApiResponse { Message = "Documento actualizado con éxito ", Success = true });
            }
            catch (Exception ex)
            {
                await _logger.Log($"Error al establecer el documento {idDocumento} como impreso: {ex.Message}");
                return BadRequest(new ApiResponse { Success = false, Message = $"No se pudo establecer el documento: {idDocumento} como impreso ", ErrorDetails = ex.Message });
            }
        }

        [HttpGet]
        [Route("/SDK/getExistencias")]
        public async Task<ActionResult<ApiResponse>> GetExistenciasSDK([FromQuery] string codigoProducto, [FromQuery] string codigoAlmacen, [FromQuery] DateTime fecha, [FromQuery] string empresa)
        {
            try
            {
                double existencias = await _getExistenciasSDK.Execute(codigoProducto, codigoAlmacen, fecha, empresa);
                return Ok(new ApiResponse { Message = $"Se encontraron {existencias} para el producto {codigoProducto} en el almacen {codigoAlmacen}", Data = existencias, Success = true });
            }
            catch (Exception ex)
            {
                await _logger.Log($"Error al obtener existencias del SDK: {ex.Message}");
                return BadRequest(new ApiResponse { Success = false, Message = "No se pudo obtener las existencias del SDK ", ErrorDetails = ex.Message });
            }
        }

        [HttpGet]
        [Route("/SDK/isGood")]
        public async Task<ActionResult<ApiResponse>> IsServiceWorkingSDK()
        {
            try
            {
                await _testSDK.Execute();
                return Ok(new ApiResponse { Message = "ContpaqiComercial-API funcionando correctamente ", Success = true });
            }
            catch (Exception ex)
            {
                await _logger.Log($"Error al probar el SDK: {ex.Message}");
                return BadRequest(new ApiResponse { Success = false, Message = "Parece que el SDK no esta funcionando correctamente :c ", ErrorDetails = ex.Message });
            }
        }
    }
}
