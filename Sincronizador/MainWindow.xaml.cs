using Core.Domain.Entities.SQL;
using Sincronizador.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        foreach (var item in PrimaryDocuments.Items)
        {
            var listViewItem = (ListViewItem)PrimaryDocuments.ItemContainerGenerator.ContainerFromItem(item);
            if (listViewItem != null)
            {
                var documento = item as DocumentoSQL;
                listViewItem.Background = _viewModel.FaltantesEnSecondary.Contains(documento)
                    ? Brushes.LightGoldenrodYellow  // Color de resaltado
                    : Brushes.White;                // Color normal
            }
        }
    }

    private void FaltantesEnSecondary_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        _viewModel.DocumentosSeleccionados = FaltantesEnSecondary.SelectedItems.Cast<DocumentoSQL>().ToList();

        if (_viewModel.DocumentosSeleccionados.Count > 0)
        {
            BtnEnviarDocumentos.Visibility = Visibility.Visible;
        }
        else
        {
            BtnEnviarDocumentos.Visibility = Visibility.Hidden;
        }
    }

    private async void BtnEnviarDocumentos_Click(object sender, RoutedEventArgs e)
    {
        foreach (var documento in _viewModel.DocumentosSeleccionados)
        {
            await _viewModel.PostDocumentoToSDK(documento);
        }
    }
}