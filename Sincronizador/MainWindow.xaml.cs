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
                DocumentoSQL? documento = item as DocumentoSQL;
                if (documento != null)
                {
                    listViewItem.Background = _viewModel.FaltantesEnSecondary.Contains(documento)
                        ? Brushes.LightGoldenrodYellow  // Color de resaltado
                        : Brushes.White;                // Color normal
                }
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
        string errorsCatched = string.Empty;
        int successCount = 0;
        int errorCount = 0;

        foreach (var documento in _viewModel.DocumentosSeleccionados)
        {
            try
            {
                await _viewModel.PostDocumentoToSDK(documento);
                successCount++;
            }
            catch (Exception ex)
            {
                errorsCatched += $"Error al enviar documento SERIE: {documento.CFOLIO} FOLIO: {documento.CSERIEDOCUMENTO} a Comercial: {ex.Message}\n";
                errorCount++;
            }
        }

        if (!string.IsNullOrEmpty(errorsCatched) && errorCount != 0)
        {
            MessageBox.Show("Error al enviar documentos a Comercial", $"Se enviaron exitosamente {successCount}, {errorCount} documentos tuvieron problemas: {errorsCatched}", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        else
        {
            MessageBox.Show($"{successCount} Documentos enviados con exito", $"Se enviaron {successCount} Documentos a Contpaqi Comercial sin errores", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}