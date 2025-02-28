using Core.Application.UseCases.Postgres;
using Core.Application.UseCases.Postgres.Movimientos;
using Core.Application.UseCases.SDK;
using Core.Application.UseCases.SQL.ClienteProveedor;
using Core.Application.UseCases.SQL.Documentos;
using Core.Application.UseCases.SQL.Productos;
using Core.Domain.Interfaces.Repositories;
using Core.Domain.Interfaces.Repositories.DTOs;
using Core.Domain.Interfaces.Repositories.SQL;
using Domain.SDK_Comercial;
using Infrastructure.Context;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Postgres;
using Infrastructure.Repositories.SQL;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Pedidos_CPE.DI
{
    public static class DependencyServices
    {
        public static void ConfigureServices(IServiceCollection services)
        {

            //builder.Host.UseWindowsService();
            //builder.Services.AddWindowsService();//to use it as a windows service

            var sdkSettings = LoadSettings();
            
            services.AddSingleton<SDKSettings>(provider => sdkSettings);
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            InjectLogging(services);

            InjectDbContexts(services, sdkSettings);

            InjectContpaqiSDK(services);


            InjectRepos(services);

            InjectSQLServices(services);

            InjectPostgresServices(services);

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        private static void InjectLogging(IServiceCollection services)
        {
            var logFilePath = Path.Combine(AppContext.BaseDirectory, "log");

            // Crea el directorio si no existe
            if (!Directory.Exists(logFilePath))
            {
                if (string.IsNullOrEmpty(logFilePath))
                {
                    throw new Exception("Directory path is empty");
                }
                Directory.CreateDirectory(logFilePath);
            }

            var logger = new Logger(Path.Combine(logFilePath, "log.txt"));

            // Add services to the container.
            services.AddSingleton<Core.Domain.Interfaces.Services.ILogger>(provider => logger);
        }

        private static void InjectDbContexts(IServiceCollection services, SDKSettings sdkSettings)
        {
            services.AddDbContext<ContpaqiSQLContext>(options =>
            {
                options.UseSqlServer(sdkSettings.SQLConnectionString,
                    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null));
            });

            services.AddDbContext<PostgresCPEContext>(options =>
            {
                options.UseNpgsql(sdkSettings.PostgresConnectionString,
                    npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorCodesToAdd: null));
            });
        }

        private static void InjectRepos(IServiceCollection services)
        {
            services.AddScoped<IProductoSQLRepo, ProductoSQLRepo>();
            services.AddScoped<IClienteProveedorSQLRepo, ClienteProveedorSQLRepo>();
            services.AddScoped<IDocumentoSQLRepo, DocumentoSQLRepo>();

            //for postgres
            services.AddScoped<IDocumentoDtoRepo, DocumentoDtoRepo>();
            services.AddScoped<IMovimientoDtoRepo, MovimientoDtoRepo>();
        }

        private static void InjectContpaqiSDK(IServiceCollection services)
        {
            services.AddSingleton<ISDKRepo, SDKRepo>();

            //use cases
            services.AddTransient<AddDocumentoYMovimientosSDKUseCase>();
            services.AddTransient<TestSDKUseCase>();
            services.AddTransient<SetDocumentoImpresoSDKUseCase>();
            services.AddTransient<GetExistenciasSDKUseCase>();
        }

        private static void InjectSQLServices(IServiceCollection services)
        {
            #region Productos

            services.AddTransient<SearchProductosByNameSQLUseCase>();
            services.AddTransient<GetProductosByIdsSQLUseCase>();
            services.AddTransient<GetProductosByCodigosSQLUseCase>();

            #endregion

            #region ClienteProveedor

            services.AddTransient<SearchClienteProveedorByNameSQLUseCase>();

            #endregion

            #region Documentos

            services.AddTransient<GetDocumentosByIdClienteAndDateSQLUseCase>();

            #endregion
        }

        private static void InjectPostgresServices(IServiceCollection services)
        {
            services.AddTransient<AddDocumentoYMovimientosDtoUseCase>();
            services.AddTransient<GetDocumentosPendientesDtoUseCase>();
            services.AddTransient<UpdateDocumentoPendienteDtoUseCase>();
            services.AddTransient<GetMovimientosByDocumentoIdPostgresUseCase>();
            services.AddTransient<UpdateMovimientosPostgresUseCase>();
        }

        private static SDKSettings LoadSettings()
        {
            try
            {
                var jsonPath = Path.Combine(AppContext.BaseDirectory, "SDKSettings.json");
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
