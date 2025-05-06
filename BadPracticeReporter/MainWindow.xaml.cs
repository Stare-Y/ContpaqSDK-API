using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BadPracticeReporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeDatePickers();
        }

        private void InitializeDatePickers()
        {
            DatePickerStartDate.SelectedDate = DateTime.Today.AddDays(-30);
            DatePickerEndDate.SelectedDate = DateTime.Today;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
        }

        private void BtnExecuteReport_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.RemovedItems.Count == 0 || e.AddedItems.Count == 0)
                    return;

                var previousDateString = e.RemovedItems[0].ToString();
                var newDateString = e.AddedItems[0].ToString();

                if (previousDateString == null || newDateString == null)
                    return;

                DateTime previousDate = DateTime.Parse(previousDateString);
                DateTime newDate = DateTime.Parse(newDateString);

                var picker = (DatePicker)sender;

                if(picker.Name.ToString() == "DatePickerEndDate")
                {
                    if (newDate < DatePickerStartDate.SelectedDate)
                    {
                        MessageBox.Show("La fecha FINAL no puede ser MENOR que la fecha INICIAL", "Advertencia", MessageBoxButton.OK);
                        picker.SelectedDate = previousDate;
                    }
                }
                else
                {
                    if (newDate > DatePickerEndDate.SelectedDate)
                    {
                        MessageBox.Show("La fecha INICIAL no puede ser MAYOR que la fecha FINAL", "Advertencia", MessageBoxButton.OK);

                        picker.SelectedDate = previousDate;
                    }
                }
                
            }   
            catch(Exception ex)
            {
                MessageBox.Show("Error", ex.Message, MessageBoxButton.OK);
            }
            finally
            {
                e.RemovedItems.Clear();
                e.AddedItems.Clear();
            }
        }
    }
}