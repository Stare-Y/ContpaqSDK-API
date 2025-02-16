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
            response.EnsureSuccessStatusCode(); // Throw if not a success code.

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(responseContent);

            return result == null ? throw new Exception("Hubo un error, la respuesta del servidor resulto nula") : result;
        }

        public async Task<T> PostAsync<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(responseContent);

            return result == null ? throw new Exception("Hubo un error, la respuesta del servidor resulto nula") : result;
        }
        public async Task<T> PutAsync<T>(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync(endpoint, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(responseContent);

            return result == null ? throw new Exception("Hubo un error, la respuesta del servidor resulto nula") : result;
        }
    }
}
