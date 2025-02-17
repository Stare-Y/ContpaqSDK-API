using Core.Domain.Entities.DTOs;

namespace Core.Domain.Interfaces.Services.ApiServices.Documentos
{
    public interface IDocumentoService
    {
        /// <summary>
        /// Obtiene la COLECCION de documentos pendientes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Lista de documentos pendientes xd</returns>
        Task<IEnumerable<DocumentoDto>> GetPendientes();

        /// <summary>
        /// Envia un documento pendiente con sus movimientos, si no, enviar la coleccion de movimientos vacia
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns>El documento creado con su id y tal</returns>
        Task<DocumentoDto> PostPendientes(DocumentoDto documento, IEnumerable<MovimientoDto> movimientoDtos);

        /// <summary>
        /// Envia un documento con sus movimientos a la API de SDK, para que aparezca completado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="movements"></param>
        /// <returns>Diccionario con id de sql y folio</returns>
        Task<Dictionary<int, double>> PostDocumentoSDK(DocumentoDto document, IEnumerable<MovimientoDto> movements);

        /// <summary>
        /// Actualiza un documento en la base de datos
        /// </summary>
        /// <param name="documento"></param>
        /// <returns></returns>
        Task PutAsync(DocumentoDto documento);
    }
}