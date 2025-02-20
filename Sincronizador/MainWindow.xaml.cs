using Core.Domain.Interfaces.Services;
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
    }

    private void btnSync_Click(object sender, RoutedEventArgs e)
    {
        
    }
}