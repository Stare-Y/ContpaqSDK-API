using ComercialSDK.Domain.Consts;
using System.Runtime.InteropServices;

namespace ComercialSDK.Domain.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct tSeriesCapas
    {
        /// <summary>
        ///     Unidades del movimiento.
        /// </summary>
        public double aUnidades;

        /// <summary>
        ///     Tipo de cambio del movimiento.
        /// </summary>
        public double aTipoCambio;

        /// <summary>
        ///     Series del movimiento.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongCodigo)]
        public string aSeries;

        /// <summary>
        ///     Pedimento del movimiento.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongDescripcion)]
        public string aPedimento;

        /// <summary>
        ///     Agencia aduanal del movimiento.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongDescripcion)]
        public string aAgencia;

        /// <summary>
        ///     Fecha de pedimento del movimiento.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongFecha)]
        public string aFechaPedimento;

        /// <summary>
        ///     Número de lote del movimiento.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongDescripcion)]
        public string aNumeroLote;

        /// <summary>
        ///     Fecha de fabricación del movimiento.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongFecha)]
        public string aFechaFabricacion;

        /// <summary>
        ///     Fecha de Caducidad del movimiento.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = ConstComercialSDK.kLongFecha)]
        public string aFechaCaducidad;
    }

}
