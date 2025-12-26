using ComercialSDK.Domain.Consts;
using System.Runtime.InteropServices;

namespace ComercialSDK.Domain.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct tMovimiento
    {
        public int aConsecutivo;
        public double aUnidades;
        public double aPrecio;
        public double aCosto;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongCodigo)]
        public string aCodProdSer;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongCodigo)]
        public string aCodAlmacen;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongReferencia)]
        public string aReferencia;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongCodigo)]
        public string aCodClasificacion;
    }
}
