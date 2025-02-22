namespace Sincronizador.Models
{
    public class SincronizadorSettings
    {
        public string? PrimaryEmpresaName { get; set; }
        public string? PrimaryConnectionString { get; set; }
        public string? SecondaryEmpresaName { get; set; }
        public string? SecondaryConnectionString { get; set; }
        public string? TargetEmpresa { get; set; }
        public string? ConceptoDefault { get; set; }
        public string? CodigoAlmacen { get; set; }
        public string? CodigoClasificacion { get; set; }
        public string? ServerUri { get; set; }
    }
}
