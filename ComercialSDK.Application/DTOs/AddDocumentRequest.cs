namespace ComercialSDK.Application.DTOs
{
    public class AddDocumentRequest
    {
        public required DocumentoDto Document { get; set; }
        public required string Empresa { get; set; }
    }
}
