namespace ComercialSDK.Application.DTOs
{
    public class AddDocumentResultDto
    {
        public int DocumentId { get; set; }
        public required string DocumentFolio { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
