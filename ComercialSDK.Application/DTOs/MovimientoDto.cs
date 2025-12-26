using ComercialSDK.Domain.Structs;

namespace ComercialSDK.Application.DTOs
{
    public record MovimientoDto
    {
        public int Id { get; set; }
        public string CodigoProducto { get; init; } = string.Empty;
        public string CodigoAlmacen { get; init; } = string.Empty;
        public string CodigoClasificacion { get; init; } = string.Empty;
        public double Unidades { get; init; }
        public double Precio { get; init; }
        public double Costo { get; init; }
        public DateTime Fecha { get; init; }
        public string Referencia { get; init; } = string.Empty;
        public double Surtidas { get; init; }
        public bool EsGranel { get; init; }
        public tMovimiento ToSDKMovimiento()
        {
            return new tMovimiento
            {
                aConsecutivo = Id,
                aUnidades = Unidades,
                aPrecio = Precio,
                aCosto = Costo,
                aCodProdSer = CodigoProducto,
                aCodAlmacen = CodigoAlmacen,
                aReferencia = Referencia,
                aCodClasificacion = CodigoClasificacion
            };
        }
    }
}
