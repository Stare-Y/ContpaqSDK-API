using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.Movimientos;
using Newtonsoft.Json;

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
            var response = await _apiService.GetAsync<ApiResponse>($"Movimientos/ByDocumentoId?documentoId={idDocumento}");

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception($"Error al buscar movimientos para el documento {idDocumento}: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var movimientos = JsonConvert.DeserializeObject<List<MovimientoDto>>(json);

            return movimientos ?? throw new Exception("Error al buscar movimientos, la instancia de respuesta fue nula.");
        }

        public Task PutUnidadesMovimientoDto(int idMovimiento, double unidades)
        {
            throw new NotImplementedException();
        }

        public async Task PatchRangeAsync(IEnumerable<MovimientoDto> movimientos)
        {
            var response = await _apiService.PatchAsync<ApiResponse>("Movimientos", movimientos);

            if (!response.Success)
                throw new Exception($"Error al actualizar los movimientos: " + response.ErrorDetails);

            return;
        }
    }
}
