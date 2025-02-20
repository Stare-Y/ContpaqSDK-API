using Core.Domain.Interfaces.Services.ApiServices;
using Core.Domain.Interfaces.Services.ApiServices.Documentos;
using Infrastructure.Context;
using Infrastructure.Services.API;
using Infrastructure.Services.API.Documentos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sincronizador.Models;
using Sincronizador.ViewModels;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace Sincronizador;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IServiceProvider _serviceProvider = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        //base.OnStartup(e);
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        _serviceProvider = serviceCollection.BuildServiceProvider();
        
        
    }

    private void ConfigureServices(ServiceCollection services)
    {
        SincronizadorSettings settings = LoadSettings();

        services.AddSingleton<MainWindow>();

        services.AddHttpClient<IApiService, ApiService>("CommonHttpClient", client =>
        {
            client.BaseAddress = new Uri(settings.ServerUri ?? throw new InvalidDataException("ServerUri de settings.json es nulo"));
            client.Timeout = TimeSpan.FromSeconds(20);
        });
        services.AddSingleton<IApiService, ApiService>();

        services.AddSingleton<IDocumentoService, DocumentoService>();

        services.AddSingleton<VMSincronizador>(provider =>
        {
            DbContextOptionsBuilder<ContpaqiSQLContext> optionsFiscal = new();
            optionsFiscal.UseSqlServer(
                settings.FiscalConnectionString,
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null));

            DbContextOptionsBuilder<ContpaqiSQLContext> optionsNoFiscal = new();
            optionsNoFiscal.UseSqlServer(
                settings.NoFiscalConnectionString,
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null));

            var documentoService = provider.GetRequiredService<IDocumentoService>();

            return new VMSincronizador(
                optionsFiscal.Options,
                optionsNoFiscal.Options,
                settings.SerieDefault ?? throw new Exception("SerieDefault de settings nula"),
                documentoService);
        });
    }

    private SincronizadorSettings LoadSettings()
    {
        var jsonPath = Path.Combine(AppContext.BaseDirectory, "Data\\settings.json");
        if (!File.Exists(jsonPath))
        {
            throw new Exception($"settings.json not found on path: {jsonPath}");
        }

        string json = File.ReadAllText(jsonPath);
        if (string.IsNullOrEmpty(json))
        {
            throw new Exception("settings.json is empty");
        }
        else
        {
            var settings = JsonSerializer.Deserialize<SincronizadorSettings>(json) ?? throw new Exception("Json settings invalido");

            if (settings.FiscalConnectionString == null || settings.NoFiscalConnectionString == null || settings.SerieDefault == null)
                throw new FileLoadException("settings.json es invalido, faltan atributos");

            return settings;
        }
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();

        mainWindow.Show();
    }
}

