using Core.Application.ViewModels.Base;
using Core.Domain.Entities;
using Core.Domain.Entities.DTOs;
using Core.Domain.Entities.SDK.Estructuras;
using Core.Domain.Interfaces.Services.ApiServices.Movimientos;
using Domain.SDK_Comercial;
using System.IO.Ports;

namespace Core.Application.ViewModels
{
    public class VMCaptureUnidades : ViewModelBase
    {
        private readonly IMovimientoService _movimientoService = null!;
        private MovimientoDto _movimiento = null!;
        private ProductoDto _producto = null!;
        BasculaSettings _basculaSettings = null!;

        public VMCaptureUnidades(IMovimientoService movimientoService, BasculaSettings basculaSettings)
        {
            _movimientoService = movimientoService;
            _basculaSettings = basculaSettings;
        }

        public VMCaptureUnidades() { }

        public MovimientoDto Movimiento
        {
            get => _movimiento;
            set
            {
                _movimiento = value;
                OnPropertyChanged(nameof(Movimiento));
            }
        }

        public ProductoDto Producto
        {
            get => _producto;
            set
            {
                _producto = value;
                OnPropertyChanged(nameof(Producto));
            }
        }

        public string LeerPeso()
        {
            ////Configuracion para RHINO BAR-6x
            //SerialPort serialPort = new SerialPort(_basculaSettings.PuertoBascula, _basculaSettings.BaudRateBascula, Parity.None, _basculaSettings.DataBitsBascula, StopBits.One);
            //serialPort.Handshake = Handshake.None;
            //serialPort.WriteTimeout = 500;
            //serialPort.ReadTimeout = 500;

            //try
            //{
            //    serialPort.Open();

            //    serialPort.Write(_basculaSettings.WriteCommandBascula);

            //    Thread.Sleep(500);

            //    // Lee la respuesta de la báscula
            //    string response = serialPort.ReadExisting();
            //    if (response != null)
            //    {
            //        response = response.Replace(_basculaSettings.SufijoBascula, "");
            //        return response.Trim();
            //    }
            //    throw new Exception("No se recibió respuesta de la báscula");
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception($"Error al leer el peso de la báscula {ex.Message}", ex);
            //}
            //finally
            //{
            //    if (serialPort.IsOpen)
            //    {
            //        serialPort.Close();
            //    }
            //}

            return "1";
        }

        public async Task UpdateMovimiento()
        {
            await _movimientoService.PatchRangeAsync(new List<MovimientoDto> { Movimiento });
        }
    }
}
