using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.Movimientos;

namespace Infrastructure.Services.API.Movimientos
{
    public class MovimientoService : IMovimientoService
    {
        private readonly IApiService _apiService;
        public MovimientoService(IApiService apiService)
        {
            _apiService = apiService;
        }
        
        public async Task<IEnumerable<MovimientoDto>> GetByDcocumentoIdAsync(int idDocumento)
        {
            var response = await _apiService.GetAsync<ApiResponse>($"Movimientos/ByDocumentoId/{idDocumento}");

            List<MovimientoDto>? movimientos = response.Data as List<MovimientoDto>;

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception($"Error al buscar movimientos para el documento {idDocumento}: " + response.ErrorDetails);

            return movimientos ?? throw new Exception("Error al buscar movimientos, la instancia de respuesta fue nula.");
        }

        public async Task PutUnidadesMovimientoDto(int idMovimiento, double unidades)
        {

            var response = await _apiService.PutAsync<ApiResponse>($"patchUnidadesMovimientoByIdSQL/{idMovimiento}/{unidades}", null);

            if (!response.Success)
                throw new Exception($"Error al actualizar las unidades del movimiento {idMovimiento}: " + response.ErrorDetails);

            return;
        }

        public async Task PatchRangeAsync(IEnumerable<MovimientoDto> movimientos)
        {
            var response = await _apiService.PostAsync<ApiResponse>("Movimientos", movimientos);

            if (!response.Success)
                throw new Exception($"Error al actualizar los movimientos: " + response.ErrorDetails);

            return;
        }

    }
}
