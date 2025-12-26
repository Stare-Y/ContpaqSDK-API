using ComercialSDK.Application.DTOs;
using ComercialSDK.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ComercialSDK.APIv2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComercialSDKController : ControllerBase
    {
        private readonly IComercialSDKService _comercialSDKService;
        public ComercialSDKController(IComercialSDKService comercialSDKService)
        {
            _comercialSDKService = comercialSDKService;
        }

        [HttpPost("Document")]
        public async Task<IActionResult> AddDocument(AddDocumentRequest request)
        {
            try
            {
                AddDocumentResult result = await _comercialSDKService.AddDocumentoAsync(request.Document, request.Empresa);

                return Ok(new GenericResponse<int> { Data = result.DocumentId, Message = string.IsNullOrEmpty(result.Notes) ? "Document Generated Successfully" : result.Notes });//TODO: build proper response object later
            }
            catch (Exception ex)
            {
                return BadRequest(new GenericResponse<object> { Data = null, Message = ex.Message });
            }
        }

        [HttpGet("Health")]
        public IActionResult HealthCheck()
        {
            _comercialSDKService.EnsureWorking();

            return Ok("ComercialSDK API is running.");
        }
    }
}
