using ComercialSDK.Application.DTOs;
using ComercialSDK.Application.Interfaces;
using ComercialSDK.Domain.Entities;
using ComercialSDK.Domain.Structs;
using Microsoft.Extensions.Options;
using Serilog;
using System.Text;
using SDK = ComercialSDK.Domain.Interfaces.ComercialSDKConnector;

namespace ComercialSDK.Infrastructure.Services
{
    public class ComercialSDKService : IComercialSDKService
    {
        private readonly ILogger _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly ComercialSDKSettings _settings;

        public ComercialSDKService(ILogger logger, IOptions<ComercialSDKSettings> options)
        {
            _logger = logger;

            _settings = options.Value;

            KickOffSDK();
        }

        public void KickOffSDK()
        {

            SDK.SetCurrentDirectory(_settings.RutaBinarios);
            SDK.SetDllDirectory(_settings.RutaBinarios);

            _logger.Information("Directorios de aplicacion Establecidos");

            int lError;

            // Verifica si la DLL existe en el directorio actual
            string dllPath = Path.Combine(_settings.RutaBinarios, "MGWServicios.dll");
            if (!File.Exists(dllPath))
            {
                throw new InvalidOperationException("No se encontro MGWServicios.dll en el directorio especificado.");
            }
            _logger.Information("DLL con SDK encontrada en el directorio especificado.");

            _logger.Information("Iniciando sesion en SDK...");

            SDK.fInicioSesionSDK(_settings.User, _settings.Password);
            
            _logger.Information("Inicio de sesion exitoso.");

            string directory = Directory.GetCurrentDirectory();

            _logger.Information($"Intentando Setear el nombre del PAQ (directorio actual: {directory})...");

            lError = SDK.fSetNombrePAQ(_settings.NombrePAQ);
            if (lError != 0)
            {
                _logger.Error($"Error al establecer el nombrePAQ: {_settings.NombrePAQ}. ({SDK.ParseErrorNumber(lError)})");
                throw new InvalidOperationException($"Error al establecer el nombrePAQ ({lError})");
            }

            _logger.Information($"NombrePAQ: {_settings.NombrePAQ} establecido con exito.");

            _logger.Information($"Intentando abrir la empresa ({_settings.EmpresaDefault})...");

            int attempts = 0;
            while (true)
            {
                lError = SDK.fAbreEmpresa(_settings.RutaEmpresas + _settings.EmpresaDefault);
                if (lError == 0)
                {
                    _logger.Information($"Empresa: {_settings.EmpresaDefault} abierta con exito.");

                    SDK.fCierraEmpresa();

                    _logger.Information("SDK inicializado correctamente.");

                    return;
                }

                if (++attempts > 4)
                {
                    _logger.Error($"No se pudo abrir la empresa default: {_settings.EmpresaDefault}. ({SDK.ParseErrorNumber(lError)})");
                    throw new InvalidOperationException($"No se pudo abrir la empresa al inicializar: {_settings.RutaEmpresas + _settings.EmpresaDefault}.");
                }

                Thread.Sleep(500);
            }
        }

        private void StartTransaction(string empresa)
        {
            int attempts = 0;
            
            while (true)
            {
                int lError = SDK.fAbreEmpresa(_settings.RutaEmpresas + empresa);
                if (lError == 0)
                {
                    _logger.Information($"Empresa: {empresa} abierta con exito, transaccion iniciada");
                    return;
                }

                if (++attempts > 4)
                {
                    _logger.Error($"No se pudo abrir la empresa: {empresa}. ({SDK.ParseErrorNumber(lError)})");
                    throw new InvalidOperationException($"No se pudo abrir la empresa: {_settings.RutaEmpresas + empresa}.");
                }

                Thread.Sleep(500);
            }
        }

        private void StopTransaction()
        {
            SDK.fCierraEmpresa();

            _logger.Information("Transacción finalizada con éxito.");
        }

        public void SetDatoDocumento(string field, string value, int idDocumento)
        {
            int lError = SDK.fBuscarIdDocumento(idDocumento);
            if (lError != 0)
            {
                throw new KeyNotFoundException($"Error buscando el documento con id: {idDocumento}: {SDK.ParseErrorNumber(lError)}");
            }

            lError = SDK.fEditarDocumento();
            if (lError != 0)
            {
                throw new Exception($"Error Cambiando estado a fEditarDocumento: {SDK.ParseErrorNumber(lError)}");
            }

            lError = SDK.fSetDatoDocumento(field, value);
            if (lError != 0)
            {
                int error = SDK.fCancelarModificacionDocumento();
                if (error != 0)
                {
                    throw new Exception($"Hubo un error ejecutando fSetDatoDocumento ({SDK.ParseErrorNumber(lError)}), y despues se intento cancelar la modificacion, lo que resulto en: {SDK.ParseErrorNumber(error)}");
                }
                throw new Exception($"Error estableciendo el valor: {value} en la columna: {field} para el documento: {idDocumento}: {SDK.ParseErrorNumber(lError)}");
            }

            lError = SDK.fGuardaDocumento();
            if (lError != 0)
            {
                int error = SDK.fCancelarModificacionDocumento();
                if (error != 0)
                {
                    throw new Exception($"Hubo un error ejecutando fGuardaDocumento ({SDK.ParseErrorNumber(lError)}), y despues se intento cancelar la modificacion, lo que resulto en: {SDK.ParseErrorNumber(error)}");
                }
                throw new Exception($"Error guardando los cambios previamente establecidos en fGuardaDocumento: {SDK.ParseErrorNumber(lError)}");
            }
        }

        public async Task EnsureWorking()
        {
            await _semaphore.WaitAsync();

            StartTransaction(_settings.EmpresaDefault);

            StopTransaction();

            _semaphore.Release();
        }

        public async Task<int> AddDocumentoAsync(DocumentoDto documentoDto, string empresa)
        {
            await _semaphore.WaitAsync();

            StartTransaction(empresa);

            try
            {
                tDocumento documentoStruct = documentoDto.ToSDKDocumento();

                double folio = 0;

                StringBuilder serie = new StringBuilder(documentoStruct.aSerie);

                int lError = SDK.fSiguienteFolio(documentoStruct.aCodConcepto, serie, ref folio);
                if(lError != 0)
                {
                    throw new InvalidOperationException($"Error al obtener siguiente folio para concepto {documentoStruct.aCodConcepto}, serie {documentoStruct.aSerie}. ({SDK.ParseErrorNumber(lError)})");
                }

                _logger.Information($"Siguiente folio obtenido: {folio} para concepto {documentoStruct.aCodConcepto}, serie {documentoStruct.aSerie}.");

                int idDocumento = 0;
                lError = SDK.fAltaDocumento(ref idDocumento, ref documentoStruct);
                if (lError != 0)
                {
                    throw new InvalidOperationException($"Error al agregar documento para concepto {documentoStruct.aCodConcepto}, serie {documentoStruct.aSerie}. ({SDK.ParseErrorNumber(lError)})");
                }

                _logger.Information($"Documento agregado exitosamente. Id SQL: {idDocumento}, Serie: {documentoStruct.aSerie}, Folio: {folio}.");

                if(documentoDto.Movimientos.Count() > 0)
                {
                    try
                    {
                        _logger.Information($"Agregando movimientos al documento Id SQL: {idDocumento}...");

                        foreach (var movimientoDto in documentoDto.Movimientos)
                        {
                            tMovimiento movimientoStruct = movimientoDto.ToSDKMovimiento();
                            int idMovimiento = 0;
                            lError = SDK.fAltaMovimiento(idDocumento, ref idMovimiento, ref movimientoStruct);
                            if (lError != 0)
                            {
                                throw new InvalidOperationException($"Error al agregar movimiento para el documento Id SQL: {idDocumento}. ({SDK.ParseErrorNumber(lError)})");
                            }
                            _logger.Information($"Movimiento agregado exitosamente al documento Id SQL: {idDocumento}. Id Movimiento: {idMovimiento}, Codigo Producto: {movimientoStruct.aCodProdSer}, Cantidad: {movimientoStruct.aUnidades}.");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error al agregar movimientos al documento Id SQL: {idDocumento}. Detalles: {ex.Message}");
                    }
                }
                else
                {
                    _logger.Warning($"No se proporcionaron movimientos para el documento Id SQL: {idDocumento}. Un documento deberia tener al menos un movimiento para ser válido.");
                }

                    try
                    {
                        //Looks hardcoded as shit, but i want to controll the specific sdk allowed fields
                        _logger.Information("Validando si hay campos extra para actualizar...");

                        if (documentoDto.cObservaciones is not null)
                        {
                            SetDatoDocumento(nameof(documentoDto.cObservaciones).ToUpper(), documentoDto.cObservaciones, idDocumento);
                        }

                        if (documentoDto.cTextoExtra1 is not null)
                        {
                            SetDatoDocumento("CTEXTOEXTRA1", documentoDto.cTextoExtra1, idDocumento);
                        }

                        if (documentoDto.cTextoExtra2 is not null)
                        {
                            SetDatoDocumento("CTEXTOEXTRA2", documentoDto.cTextoExtra2, idDocumento);
                        }

                        if (documentoDto.cTextoExtra3 is not null)
                        {
                            SetDatoDocumento("CTEXTOEXTRA3", documentoDto.cTextoExtra3, idDocumento);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error al actualizar campos extra para el documento Id SQL: {idDocumento}. Detalles: {ex.Message}");
                    }

                return idDocumento;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error al agregar documento: {ex.Message}");
                throw;
            }
            finally
            {
                StopTransaction();

                _semaphore.Release();
            }
        }
    }
}
