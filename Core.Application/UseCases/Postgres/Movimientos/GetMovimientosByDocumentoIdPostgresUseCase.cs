using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Repositories.DTOs;
using Core.Domain.Interfaces.Services;

namespace Core.Application.UseCases.Postgres.Movimientos
{
    public class GetMovimientosByDocumentoIdPostgresUseCase
    {
        private readonly IMovimientoDtoRepo _movimientoRepo;
        private readonly ILogger _logger;

        public GetMovimientosByDocumentoIdPostgresUseCase(IMovimientoDtoRepo movimientoRepo, ILogger logger)
        {
            _movimientoRepo = movimientoRepo;
            _logger = logger;
        }

        public async Task<IEnumerable<MovimientoDto>> Execute(int id)
        {
            await _logger.Log($"Obteniendo movimientos por id de documento {id}");
            return await _movimientoRepo.GetByDocumentoDtoIdAsync(id);
        }
    }
}
