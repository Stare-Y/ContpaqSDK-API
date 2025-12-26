using ComercialSDK.Application.DTOs;
using ComercialSDK.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ComercialSDK.APIv2.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
            int documentId = await _comercialSDKService.AddDocumentoAsync(request.Document, request.Empresa);
            
            return Ok(documentId);//TODO: build proper response object later
        }

        [HttpGet("Health")]
        public IActionResult HealthCheck()
        {
            _comercialSDKService.EnsureWorking();

            return Ok("ComercialSDK API is running.");
        }
    }
}
