using ApplicationLayer.ViewModels;
using CommunityToolkit.Maui;
using Domain.Interfaces.Services.ApiServices.ClientesProveedores;
using Domain.Interfaces.Services.ApiServices.Documentos;
using Domain.Interfaces.Services.ApiServices.Movimientos;
using Domain.Interfaces.Services.ApiServices.Productos;
using Domain.SDK_Comercial;
using Infrastructure.Services.API.ClientesProveedores;
using Infrastructure.Services.API.Documentos;
using Infrastructure.Services.API.Movimientos;
using Infrastructure.Services.API.Productos;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace PedidosCPE
{
    public static class MauiProgram
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            ConfigureServices(builder);

            ServiceProvider = builder.Services.BuildServiceProvider();

            return builder.Build();
        }

        private static void ConfigureServices(MauiAppBuilder builder)
        {
            var sDKSettings = LoadSettings();

            builder.Services.AddSingleton<SDKSettings>(provider => sDKSettings);

            builder.Services.AddHttpClient("CommonHttpClient", client =>
            {
                client.BaseAddress = new Uri(sDKSettings.ServerUri);
                client.Timeout = TimeSpan.FromSeconds(20);
            });

            // Registrar los servicios e inyectar el HttpClient común
            builder.Services.AddTransient<IDocumentoService, DocumentoService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return new DocumentoService(httpClientFactory.CreateClient("CommonHttpClient"));
            });

            builder.Services.AddTransient<IProductoService, ProductoService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return new ProductoService(httpClientFactory.CreateClient("CommonHttpClient"));
            });

            builder.Services.AddTransient<IClienteProveedorService, ClienteProveedorService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return new ClienteProveedorService(httpClientFactory.CreateClient("CommonHttpClient"));
            });

            builder.Services.AddTransient<IMovimientoService, MovimientoService>(sp =>
            {
                var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
                return new MovimientoService(httpClientFactory.CreateClient("CommonHttpClient"));
            });

            builder.Services.AddTransient<VMSearchProductos>();
            builder.Services.AddTransient<VMCreateDocumento>();
            builder.Services.AddTransient<VMSearchClienteProveedor>();
            builder.Services.AddTransient<VMDispatchDocumentosPendientes>();
            builder.Services.AddTransient<VMCaptureUnidades>();
            builder.Services.AddTransient<VMUnidadesPopup>();
        }

        private static SDKSettings LoadSettings()
        {
            try
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data/SDKSettings.json");
                if (!File.Exists(jsonPath))
                {
                    throw new Exception($"SDKSettings.json not found on path: {jsonPath}");
                }

                string json = File.ReadAllText(jsonPath);
                if (string.IsNullOrEmpty(json))
                {
                    throw new Exception("SDKSettings.json is empty");
                }
                else
                {
                    return JsonSerializer.Deserialize<SDKSettings>(json) ?? throw new Exception("Json SDKSettings invalido");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
