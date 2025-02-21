using Sincronizador.ViewModels;
using System.Windows;

namespace Sincronizador;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly VMSincronizador _viewModel = null!;
    public MainWindow(VMSincronizador viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        DateFechaInicio.DisplayDateEnd = DateTime.Today.AddDays(-1);
        DateFechaFin.DisplayDateEnd = DateTime.Today;
    }

    private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.GetDocumentosFiltrados();
    }
}