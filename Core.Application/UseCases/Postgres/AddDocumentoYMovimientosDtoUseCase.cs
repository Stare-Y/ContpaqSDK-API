using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Repositories.DTOs;
using Core.Domain.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.UseCases.Postgres
{
    public class AddDocumentoYMovimientosDtoUseCase
    {
        private readonly IDocumentoDtoRepo _documentoDtoRepo;
        private readonly IMovimientoDtoRepo _movimientoDtoRepo;
        private readonly ILogger _logger;

        public AddDocumentoYMovimientosDtoUseCase(IDocumentoDtoRepo documentoDtoRepo, IMovimientoDtoRepo movimientoDtoRepo, ILogger logger)
        {
            _documentoDtoRepo = documentoDtoRepo;
            _logger = logger;
            _movimientoDtoRepo = movimientoDtoRepo;
        }

        public async Task<DocumentoDto> Execute(DocumentoDto documentoDto, List<MovimientoDto> movimientoDtos)
        {
            await _logger.Log("Ejecutando caso de uso AddDocumentoYMovimientosDto...");

            var addedDocument = await _documentoDtoRepo.AddAsync(documentoDto);

            await _logger.Log($"Documento agregado a postgres con éxito. ID: {addedDocument.IdPostgres}");

            foreach (var movimiento in movimientoDtos)
            {
                movimiento.IdDocumento = addedDocument.IdPostgres;
                await _movimientoDtoRepo.AddAsync(movimiento);
            }

            await _logger.Log($"Se agregaron {movimientoDtos.Count} movimientos para el documento {addedDocument.IdPostgres}");

            return addedDocument;
        }
    }
}
