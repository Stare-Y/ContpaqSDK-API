using ComercialSDK.Domain.Consts;
using System.Runtime.InteropServices;

namespace ComercialSDK.Domain.Structs
{
    /// <summary>
    /// Struct de tipo Documento que provee el sdk
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct tDocumento
    {
        public double aFolio;
        public int aNumMoneda;
        public double aTipoCambio;
        public double aImporte;
        public double aDescuentoDoc1;
        public double aDescuentoDoc2;
        public int aSistemaOrigen;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongCodigo)]
        public string aCodConcepto;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongSerie)]
        public string aSerie;
        /// <summary>
        /// REQUIRED Format: "MM/dd/yyyy"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongFecha)]
        public string aFecha;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongCodigo)]
        public string aCodigoCteProv;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongCodigo)]
        public string aCodigoAgente;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongReferencia)]
        public string aReferencia;
        public int aAfecta;
        public double aGasto1;
        public double aGasto2;
        public double aGasto3;

    }
}
