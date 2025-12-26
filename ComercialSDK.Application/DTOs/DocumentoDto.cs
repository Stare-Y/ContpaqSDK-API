using ComercialSDK.Domain.Structs;

namespace ComercialSDK.Application.DTOs;

public record DocumentoDto
{
    public int NumMoneda { get; init; } = 0;
    public double TipoCambio { get; init; } = 0;
    public double Importe { get; init; } = 0;
    public double DescuentoDoc1 { get; init; } = 0;
    public double DescuentoDoc2 { get; init; } = 0;
    public int SistemaOrigen { get; init; } = 0;
    public required string CodConcepto { get; init; }
    public required string Serie { get; init; }
    /// <summary>
    /// REQUIRED Format: "MM/dd/yyyy"
    /// </summary>
    public required DateTime Fecha { get; init; } 
    public required string CodigoCteProv { get; init; } 
    public string RazonSocial { get; init; } = string.Empty;
    public string CodigoAgente { get; init; } = string.Empty;
    public string Referencia { get; init; } = string.Empty;
    public int Afecta { get; init; } = 0;
    public double Gasto1 { get; init; } = 0;
    public double Gasto2 { get; init; } = 0;
    public double Gasto3 { get; init; } = 0;
    public string? cObservaciones { get; init; } 
    public string? cTextoExtra1 { get; init; } 
    public string? cTextoExtra2 { get; init; } 
    public string? cTextoExtra3 { get; init; }
    public MovimientoDto[] Movimientos { get; init; } = [];
    public tDocumento ToSDKDocumento()
    {
        return new tDocumento()
        {
            aFolio = 0,
            aNumMoneda = NumMoneda,
            aTipoCambio = TipoCambio,
            aImporte = Importe,
            aDescuentoDoc1 = DescuentoDoc1,
            aDescuentoDoc2 = DescuentoDoc2,
            aSistemaOrigen = SistemaOrigen,
            aCodConcepto = CodConcepto,
            aSerie = Serie,
            aFecha = Fecha.ToString("MM/dd/yyyy"),
            aCodigoCteProv = CodigoCteProv,
            aCodigoAgente = CodigoAgente,
            aReferencia = Referencia,
            aAfecta = Afecta,
            aGasto1 = Gasto1,
            aGasto2 = Gasto2,
            aGasto3 = Gasto3
        };
    }
}
