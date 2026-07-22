using System.ComponentModel;
using System.IO;
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

namespace ProZapCommunication.UI
{
    public partial class MainWindow : Window
    {
        private readonly Bootstrapper _appEngine;
        private string BuildDate;

        public MainWindow()
        {
            InitializeComponent();
            BuildDate = File.GetLastWriteTime(
            System.Reflection.Assembly.GetExecutingAssembly().Location)
        .ToString("yyyy-MM-dd HH:mm");

            BuildDateTextBlock.Text = "Version " + Environment.NewLine + BuildDate;


            // Tworzymy instancję silnika aplikacji
            _appEngine = new Bootstrapper();

            // Uruchomienie komunikacji i maszyn stanów w tle
            _appEngine.InitializeAndStart();

            this.DataContext = _appEngine.MainVm;

            this.Closing += OnClosing;
        }


        private void BtnSendTask_Click(object sender, RoutedEventArgs e)
        {
            // Przykład wywołania: Operator klika przycisk w UI, co powoduje wrzucenie 
            // zadania (np. kodu 12) do kolejki FIFO dla PLC. Logika stacji zajmie się resztą.
            // W pełnej wersji MVVM wywołamy to przez Command powiązany z odpowiednim procesorem.
        }


        private void OnClosing(object sender, CancelEventArgs e)
        {

            var result = MessageBox.Show(
                "Czy na pewno chcesz zamknąć aplikację?",
                "Potwierdzenie",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }


            // jedno wspólne miejsce
            _appEngine.ShutDown();

        }

    }
}