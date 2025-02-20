using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices;
using Newtonsoft.Json;
using System.Text;

namespace Infrastructure.Services.API
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _client;

        public ApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<T> GetAsync<T>(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);

            await ValidateResponse(response);

            return await DeserializeResponse<T>(response);
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            StringContent content = await SerializeContent(data);

            var response = await _client.PostAsync(endpoint, content);

            await ValidateResponse(response);

            return await DeserializeResponse<T>(response);
        }

        public async Task<T> PutAsync<T>(string endpoint, object? data)
        {
            StringContent content = await SerializeContent(data);

            var response = await _client.PutAsync(endpoint, content);

            await ValidateResponse(response);

            return await DeserializeResponse<T>(response);
        }

        public async Task<T> PatchAsync<T>(string endpoint, object data)
        {
            StringContent content = await SerializeContent(data);

            var response = await _client.PatchAsync(endpoint, content);

            await ValidateResponse(response);

            return await DeserializeResponse<T>(response);
        }

        private async Task<StringContent> SerializeContent(object? data)
        {
            return await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(data);
                return new StringContent(json, Encoding.UTF8, "application/json");
            });
        }

        private async Task ValidateResponse(HttpResponseMessage response)
        {
            var apiResponseDetails = await DeserializeResponse<ApiResponse>(response);

            if (!apiResponseDetails.Success)
            {
                throw new HttpRequestException(apiResponseDetails.Message + " - " + apiResponseDetails.ErrorDetails);
            } // Throw if not a success code.
        }

        private async Task<T> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(responseContent);
            return result ?? throw new Exception("Hubo un error, la respuesta del servidor resulto nula");
        }
    }
}
