using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices.SDK;
using Microsoft.Extensions.DependencyInjection;

namespace ComercialAPITest
{
    public class Tests : IDisposable
    {
        private const string TARGET_EMPRESA = "adJOSE_DE_JESUS_MARQUE";
        private const int HOW_MANY_REQUESTS = 10;

        private readonly IServiceProvider _serviceProvider;
        private DocumentoDto _documentoDto = null!;
        private MovimientoDto _movimientoDto = null!;

        public Tests()
        {
            var serviceCollection = new ServiceCollection();
            DependencyInjection.ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task aTestSDKIsGood()
        {
            ISDKService sDKService = _serviceProvider.GetRequiredService<ISDKService>();

            Assert.IsTrue(await sDKService.IsSDKGood());
        }

        [Test]
        public async Task bSendOneDocument()
        {
            BuildDefaultDocumentoDto();
            BuildDefaultMovimientoDto();

            ISDKService sDKService = _serviceProvider.GetRequiredService<ISDKService>();

            var result = await sDKService.PostDocumentoSDK(_documentoDto, new List<MovimientoDto> { _movimientoDto }, TARGET_EMPRESA);

            Assert.That(result.Keys.First(), Is.Not.EqualTo(0));
            Assert.That(result[result.Keys.First()], Is.Not.EqualTo(0));
        }

        [Test]
        public async Task cSendMultipleDocuments()
        {
            BuildDefaultDocumentoDto();
            BuildDefaultMovimientoDto();

            ISDKService sDKService = _serviceProvider.GetRequiredService<ISDKService>();

            var tasks = new Task<Dictionary<int, double>>[HOW_MANY_REQUESTS];

            for (int i = 0; i < HOW_MANY_REQUESTS; i++)
            {
                tasks[i] = sDKService.PostDocumentoSDK(_documentoDto, new List<MovimientoDto> { _movimientoDto }, TARGET_EMPRESA);
            }

            var results = await Task.WhenAll(tasks);

            foreach(var result in results)
            {
                Assert.That(result.Keys.First(), Is.Not.EqualTo(0));
                Assert.That(result[result.Keys.First()], Is.Not.EqualTo(0));
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private void BuildDefaultDocumentoDto()
        {
            _documentoDto = new DocumentoDto
            {
                CodConcepto = "21",
                CodigoAgente = "",
                CodigoCteProv = "POLITUBO",
                Fecha = DateTime.Now.ToString("MM/dd/yyyy"),
                Impreso = false,
                Serie = "A4",
                SistemaOrigen = 205,
                Surtido = false
            };
        }

        private void BuildDefaultMovimientoDto()
        {
            _movimientoDto = new MovimientoDto
            {
                CodigoAlmacen = "1",
                CodigoProducto = "F077",
                Fecha = DateTime.Now,
                Precio = 7.7,
                Unidades = 60
            };
        }
    }
}