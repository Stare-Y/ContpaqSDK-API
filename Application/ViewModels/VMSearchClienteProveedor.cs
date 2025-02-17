using Core.Application.ViewModels.Base;
using Core.Domain.Entities.DTOs;
using Core.Domain.Interfaces.Services.ApiServices.ClienteProveedor;
using System.Collections.ObjectModel;

namespace Core.Application.ViewModels
{
    public class VMSearchClienteProveedor : ViewModelBase
    {
        private readonly IClienteProveedorService _clienteProveedorService = null!;
        private ObservableCollection<ClienteProveedorDto> _clienteProveedores = new();
        public ObservableCollection<ClienteProveedorDto> ClientesProveedoresEncontrados
        {
            get { return _clienteProveedores; }
            set { _clienteProveedores = value; OnCollectionChanged(nameof(ClientesProveedoresEncontrados)); }
        }

        private ClienteProveedorDto _clienteProveedorSeleccionado = new();
        public ClienteProveedorDto ClienteProveedorSeleccionado
        {
            get { return _clienteProveedorSeleccionado; }
            set { _clienteProveedorSeleccionado = value; OnPropertyChanged(nameof(_clienteProveedorSeleccionado)); }
        }

        public VMSearchClienteProveedor(IClienteProveedorService clienteProveedorService)
        {
            _clienteProveedorService = clienteProveedorService;
        }
        public VMSearchClienteProveedor() { }

        public async Task SearchClienteProveedorAsync(string search)
        {
            var clientesProveedores = await _clienteProveedorService.SearchAsync(search);

            ClientesProveedoresEncontrados = new ObservableCollection<ClienteProveedorDto>(clientesProveedores);

            OnCollectionChanged(nameof(ClientesProveedoresEncontrados));
        }
    }
}
