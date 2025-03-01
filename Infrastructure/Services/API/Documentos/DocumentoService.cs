using Core.Application.UseCases.Postgres.Requests;
using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Newtonsoft.Json;

namespace Infrastructure.Services.API.Documentos
{
    public class DocumentoService : IDocumentoService
    {
        private readonly IApiService _apiService;
        public DocumentoService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IEnumerable<DocumentoDto>> GetPendientes()
        {
            var response = await _apiService.GetAsync<ApiResponse>("/Pendientes");

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new FileNotFoundException("Error al obtener documentos pendientes: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var documentos = JsonConvert.DeserializeObject<List<DocumentoDto>>(json);

            return documentos ?? throw new Exception("Error al obtener documentos pendientes, la instancia de respuesta fue nula.");
        }

        public async Task<DocumentoDto> PostPendientes(DocumentoDto documento, IEnumerable<MovimientoDto> movimientoDtos)
        {
            var request = new AddDocumentoYMovimientosDtoRequest
            {
                Documento = documento,
                Movimientos = [.. movimientoDtos]
            };

            var response = await _apiService.PostAsync<ApiResponse>("/Pendientes", request);

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception("Error al crear documento pendiente: " + response.ErrorDetails);

            // Deserialize manually
            var json = JsonConvert.SerializeObject(response.Data);
            var documentoCreado = JsonConvert.DeserializeObject<DocumentoDto>(json);

            return documentoCreado ?? throw new Exception("Error al crear documento pendiente, la instancia de respuesta fue nula.");
        }

        public async Task PutAsync(DocumentoDto documento)
        {
            var response = await _apiService.PutAsync<ApiResponse>("/Pendientes", documento);

            if (!string.IsNullOrEmpty(response.ErrorDetails) && !response.Success)
                throw new Exception("Error al actualizar documento pendiente: " + response.ErrorDetails);
        }
        //public async Task<DocumentDto> GetByConceptoSerieAndFolioSDKAsync<DocumentDto>(string codConcepto, string serie, string folio)
        //{

        //    ApiResponse result = await _apiService.GetAsync<ApiResponse>($"/getDocumentByConceptoFolioAndSerieSDK/{codConcepto}/{serie}/{folio}");



        //    if (response.IsSuccessStatusCode)
        //    {
        //        var content = await response.Content.ReadAsStringAsync();
        //        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

        //        if (apiResponse.Success)
        //        {
        //            var document = JsonConvert.DeserializeObject<DocumentDto>(apiResponse.Data.ToString());
        //            return document;
        //        }
        //        else
        //        {
        //            throw new Exception(apiResponse.Message + apiResponse.ErrorDetails);
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Parece que no tuvimos una respuesta Exitosa :c: " + response.ReasonPhrase);
        //    }
        //}

        //public async Task<DocumentDTO> GetDocumentByIdSDKAsync<DocumentDTO>(int idDocumento)
        //{
        //    try
        //    {
        //        var response = await _client.GetAsync($"/getDocumentByIdSDK/{idDocumento}");
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

        //            if (apiResponse.Success)
        //            {
        //                var document = JsonConvert.DeserializeObject<DocumentDTO>(apiResponse.Data.ToString());
        //                return document;
        //            }
        //            else
        //            {
        //                throw new Exception("Parece que no tuvimos una respuesta Exitosa :c: " + apiResponse.Message);
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception("Parece que no tuvimos una respuesta Exitosa :c: " + response.ReasonPhrase);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error al obtener el documento: " + ex.Message);
        //    }
        //}

        //public async Task<List<DocumentDTO>> GetPedidosByFechaSerieCPESQL<DocumentDTO>(DateTime fechaInicio, DateTime fechaFin, string serie)
        //{
        //    try
        //    {
        //        // Formatear las fechas en formato ISO 8601 para que el API las pueda interpretar
        //        string fechaInicioFormatted = fechaInicio.ToString("yyyy-MM-ddTHH:mm:ss");
        //        string fechaFinFormatted = fechaFin.ToString("yyyy-MM-ddTHH:mm:ss");

        //        string url = $"/getPedidosByFechaSerieCPESQL/{Uri.EscapeDataString(fechaInicioFormatted)}/{Uri.EscapeDataString(fechaFinFormatted)}/{Uri.EscapeDataString(serie)}";
        //        var response = await _client.GetAsync(url);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
        //            if (apiResponse.Success)
        //            {
        //                var documents = JsonConvert.DeserializeObject<List<DocumentDTO>>(apiResponse.Data.ToString());
        //                return documents;
        //            }
        //            else
        //            {
        //                throw new Exception("Parece que no tuvimos una respuesta Exitosa para la lista de documentos :c: " + apiResponse.Message);
        //            }
        //        }
        //        else
        //        {
        //            throw new Exception($"Status Code: {response.StatusCode}, " + response.ReasonPhrase);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error al obtener la lista de documentos: " + ex.Message);
        //    }
        //}
    }
}
