namespace ComercialSDK.Application.DTOs
{
    public record ContpaqiComercialResult
    {
        public int ResultingId { get; init; }
        public string? ResultingFolio { get; init; }
        public string Message { get; init; } = "No message.";
    }
}
