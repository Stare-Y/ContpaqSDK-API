namespace Core.Domain.Entities
{
    public class BasculaSettings
    {
        public string PuertoBascula { get; set; } = string.Empty;
        public int BaudRateBascula { get; set; }
        public int DataBitsBascula { get; set; }
        public string WriteCommandBascula { get; set; } = string.Empty;
        public string SufijoBascula { get; set; } = string.Empty;
    }
}
