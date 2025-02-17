using CommunityToolkit.Maui;
using Core.Application.ViewModels;
using Core.Domain.Entities;
using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.ClienteProveedor;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Core.Domain.Interfaces.Services.ApiServices.Movimientos;
using Core.Domain.Interfaces.Services.ApiServices.Productos;
using Infrastructure.Services.API;
using Infrastructure.Services.API.CLienteProveedor;
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

            var app = builder.Build();
            ServiceProvider = app.Services; // ✅ Correct way to get it

            return app;
        }

        private static void ConfigureServices(MauiAppBuilder builder)
        {
            TerminalSettings terminalSettings = LoadTerminalSettings();
            BasculaSettings basculaSettings = LoadBasculaSettings();

            builder.Services.AddSingleton(LoadTerminalSettings());
            builder.Services.AddSingleton(LoadBasculaSettings());

            builder.Services.AddHttpClient<IApiService, ApiService>("CommonHttpClient", client =>
            {
                client.BaseAddress = new Uri(terminalSettings.ServerUri);
                client.Timeout = TimeSpan.FromSeconds(20);
            });
            builder.Services.AddTransient<IProductoService, ProductoService>();

            builder.Services.AddTransient<IClienteProveedorService, ClienteProveedorService>();

            builder.Services.AddTransient<IMovimientoService, MovimientoService>();

            builder.Services.AddTransient<IDocumentoService, DocumentoService>();

            builder.Services.AddTransient<VMSearchProductos>();
            builder.Services.AddTransient<VMCreateDocumento>();
            builder.Services.AddTransient<VMSearchClienteProveedor>();
            builder.Services.AddTransient<VMDispatchDocumentosPendientes>();
            builder.Services.AddTransient<VMCaptureUnidades>();
            builder.Services.AddTransient<VMUnidadesPopup>();
        }

        private static TerminalSettings LoadTerminalSettings()
        {
            try
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data\\TerminalSettings.json");
                if (!File.Exists(jsonPath))
                {
                    throw new Exception($"TerminalSettings.json not found on path: {jsonPath}");
                }

                string json = File.ReadAllText(jsonPath);
                if (string.IsNullOrEmpty(json))
                {
                    throw new Exception("TerminalSettings.json is empty");
                }
                else
                {
                    return JsonSerializer.Deserialize<TerminalSettings>(json) ?? throw new Exception("Json TerminalSettings invalido");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static BasculaSettings LoadBasculaSettings()
        {
            try
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data\\BasculaSettings.json");
                if (!File.Exists(jsonPath))
                {
                    throw new Exception($"BasculaSettings.json not found on path: {jsonPath}");
                }
                string json = File.ReadAllText(jsonPath);
                if (string.IsNullOrEmpty(json))
                {
                    throw new Exception("BasculaSettings.json is empty");
                }
                else
                {
                    return JsonSerializer.Deserialize<BasculaSettings>(json) ?? throw new Exception("Json BasculaSettings invalido");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
