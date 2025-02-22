using Core.Domain.Entities.SQL;
using Sincronizador.ViewModels;
using Sincronizador.Views;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Sincronizador;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private LoadingWindow _loadingWindow;

    private readonly VMSincronizador _viewModel = null!;
    public MainWindow(VMSincronizador viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
        DateFechaInicio.DisplayDateEnd = DateTime.Today.AddDays(-1);
        DateFechaFin.DisplayDateEnd = DateTime.Today;
        _loadingWindow = new();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _loadingWindow.Close();
        Application.Current.Shutdown();
    }

    private async void BtnBuscar_Click(object sender, RoutedEventArgs e)
    {
        await GetDocumentos();
    }

    private async Task GetDocumentos()
    {
        _loadingWindow.Show();


        await _viewModel.GetDocumentosFiltrados();

        await Task.Delay(1000);

        HighlightListDifferences();

        _loadingWindow.Hide();
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
        if (_viewModel.DocumentosSeleccionados.Count == 0)
        {
            MessageBox.Show("No hay documentos seleccionados para enviar, elije almenos uno por favor", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        //display a message box to confirm if we want to send the documents
        MessageBoxResult result = MessageBox.Show($"Se enviaran {_viewModel.DocumentosSeleccionados.Count} documentos a Comercial\n\nEstas de acuedo?", "Confirmar envio de documentos", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.No)
        {
            return;
        }

        ShowProgressBar();

        string errorsCatched = string.Empty;
        int successCount = 0;
        int errorCount = 0;
        int total = _viewModel.DocumentosSeleccionados.Count;

        foreach (var documento in _viewModel.DocumentosSeleccionados)
        {
            try
            {
                await _viewModel.PostDocumentoToSDK(documento);
                successCount++;
            }
            catch (Exception ex)
            {
                errorsCatched += $"Error al enviar documento SERIE: {documento.CFOLIO} FOLIO: {documento.CSERIEDOCUMENTO} a Comercial: {ex.Message}\n\n";
                errorCount++;
            }
            finally
            {
                UpdateProgressBar(successCount + errorCount, total);
            }
        }


        if (!string.IsNullOrEmpty(errorsCatched) && errorCount != 0)
        {
            MessageBox.Show($"Se enviaron exitosamente {successCount}, pero {errorCount} documentos tuvieron problemas:\n{errorsCatched}", "Hubieron algunos errores", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        else
        {
            MessageBox.Show($"Se enviaron {successCount} Documentos a Contpaqi Comercial sin errores", $"{successCount} Documentos enviados con exito", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        await GetDocumentos();

        HighlightListDifferences();
    }

    private void UpdateProgressBar(int itemsGoing, int total)
    {
        int progress = (itemsGoing * 100) / total;
        ProgressBarEnvio.Value = progress;
        TxtProgress.Text = $"{progress}%";

        if (progress == 100)
        {
            HideProgressBar();
        }
    }

    private void ShowProgressBar()
    {
        ProgressBarEnvio.Visibility = Visibility.Visible;
        TxtProgress.Visibility = Visibility.Visible;
        ProgressBarEnvio.Value = 0;
        TxtProgress.Text = "0%";
    }

    private void HideProgressBar()
    {
        ProgressBarEnvio.Visibility = Visibility.Hidden;
        TxtProgress.Visibility = Visibility.Hidden;
    }

    private void HighlightListDifferences()
    {
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
}