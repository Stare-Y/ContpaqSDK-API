using ComercialSDK.Application.DTOs;

namespace ComercialSDK.Application.Interfaces;

public interface IComercialSDKService
{
    /// <summary>
    /// Remember, documents must have at least 1 movement to be valid
    /// </summary>
    /// <param name="documentoDto"></param>
    /// <returns>
    /// The PK of the created document in the SQL Database
    /// </returns>
    Task<int> AddDocumentoAsync(DocumentoDto documentoDto, string empresa);

    Task EnsureWorking();
}
