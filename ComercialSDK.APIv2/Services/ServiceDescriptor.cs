using ComercialSDK.Application.Interfaces;
using ComercialSDK.Domain.Entities;
using ComercialSDK.Infrastructure.Services;

namespace ComercialSDK.APIv2.Services
{
    public static class ServiceDescriptor
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ComercialSDKSettings>(configuration.GetSection(nameof(ComercialSDKSettings)));
            services.AddSingleton<IComercialSDKService, ComercialSDKService>();
            return services;
        }
    }
}
