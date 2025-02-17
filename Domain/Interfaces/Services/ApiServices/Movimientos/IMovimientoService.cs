using Core.Domain.Entities.DTOs;

namespace Core.Domain.Interfaces.Services.ApiServices.Movimientos
{
    public interface IMovimientoService
    {
        /// <summary>
        /// Obtiene los movimientos de un documento por su id
        /// </summary>
        /// <param name="idDocumento"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        Task<IEnumerable<MovimientoDto>> GetByDcocumentoIdAsync(int idDocumento);

        /// <summary>
        /// Updates the movement with the provided id, with the provided unidades
        /// </summary>
        /// <param name="idMovimiento"></param>
        /// <param name="unidades"></param>
        /// <returns>Completed task and the message form the api</returns>
        Task PutUnidadesMovimientoDto(int idMovimiento, double unidades);

        /// <summary>
        /// Updates the movement with the provided id, with the provided unidades
        /// </summary>
        /// <param name="movimientos"></param>
        /// <returns></returns>
        Task PatchRangeAsync(IEnumerable<MovimientoDto> movimientos);

        /// <summary>
        /// Agrega un rango de movimientos pertenecientes a un documento ya valido
        /// </summary>
        /// <param name="movimientos"></param>
        /// <returns></returns>
        Task PostRangeAsync(IEnumerable<MovimientoDto> movimientos);

    }
}
