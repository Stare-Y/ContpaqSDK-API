using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.SDK;
using Infrastructure.Services.API;
using Infrastructure.Services.API.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace ComercialAPITest
{
    public static class DependencyInjection
    {
        private const string API_SERVER_URI = "http://26.116.39.19:6969/";
        public static void ConfigureServices(IServiceCollection services)
        {
            InjectHttpClient(services);

            InjectAPIServices(services);
        }

        private static void InjectHttpClient(IServiceCollection services)
        {
            services.AddHttpClient<IApiService, ApiService>("CommonHttpClient", client =>
            {
                client.BaseAddress = new Uri(API_SERVER_URI ?? throw new InvalidDataException("ServerUri de settings.json es nulo"));
                client.Timeout = TimeSpan.FromSeconds(40);
            });
        }

        private static void InjectAPIServices(IServiceCollection services)
        {
            services.AddSingleton<ISDKService, SDKService>();

        }
    }
}
