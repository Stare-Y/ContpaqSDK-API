using Core.Domain.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Interfaces.Services.ApiServices.SDK
{
    public interface ISDKService
    {
        /// <summary>
        /// Obtiene las existencias de un producto en un almacen en una fecha determinada
        /// </summary>
        /// <remarks>BUENA PRACTICA: que fecha siempre sea hoy, para evitar pedos xD</remarks>
        /// <param name="codigoProducto"></param>
        /// <param name="codigoAlmacen"></param>
        /// <param name="fecha"></param>
        /// <returns>las existencias del producto solicitado</returns>
        Task<double> GetExistenciasAsync(string codigoProducto, string codigoAlmacen, DateTime fecha);

        /// <summary>
        /// Envia un documento con sus movimientos a la API de SDK, para que aparezca completado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="movements"></param>
        /// <returns>Diccionario con id de sql y folio</returns>
        Task<Dictionary<int, double>> PostDocumentoSDK(DocumentoDto document, IEnumerable<MovimientoDto> movements, string empresa);

        Task<bool> IsSDKGood();
    }
}
