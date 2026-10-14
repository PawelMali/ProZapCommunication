using Serilog;
using System.IO;
using System.Windows;

namespace ProZapCommunication.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool IsForcedShutdown { get; private set; }
        private static Mutex _mutex = null;
        private const string AppName = "ProZapCommunication2026";


        protected override void OnStartup(StartupEventArgs e)
        {
            // Pobieramy ścieżkę do folderu z logami w katalogu uruchomieniowym aplikacji
            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

            // KONFIGURACJA SERILOGA
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()// Logujemy wszystko od poziomu Debug wzwyż
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Map<string>(
                    keyPropertyName: "StationIdentity", // NOWY KLUCZ MAPOWANIA: Identyfikator Stacji
                    defaultKey: "MainApplication",
                    configure: (stationIdentity, wt) =>
                    {
                        // Definiujemy ścieżkę do pliku logu stacji, np. Logs/STACJA_1_WKRECANIE/station_log-.txt
                        string logPath = stationIdentity.Equals("MainApplication")
                            ? Path.Combine(logFolder, "app_main-.txt")
                            : Path.Combine(logFolder, $"{stationIdentity}", "station_log-.txt");

                        wt.File(
                            path: logPath,
                            rollingInterval: RollingInterval.Day,
                            rollOnFileSizeLimit: true,
                            fileSizeLimitBytes: 10 * 1024 * 1024, // 10 MB
                            retainedFileCountLimit: 60,
                            outputTemplate: "[{Timestamp:HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                        );
                    })
                .CreateLogger();

            Log.Information("Serilog logging system initialized successfully.");

            bool createdNew;

            _mutex = new Mutex(true, AppName, out createdNew);

            if (!createdNew)
            {
                // aplikacja już działa
                IsForcedShutdown = true;
                MessageBox.Show("Aplikacja jest już uruchomiona!", "UWAGA!", MessageBoxButton.OK, MessageBoxImage.Warning);

                Shutdown();
                return;
            }


            if (createdNew)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
            }


            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("Closing ProZapCommunication application. Flushing logs...");
            Log.CloseAndFlush(); // Krytyczne: wymusza zapisanie buforowanych logów do plików na dysku
            base.OnExit(e);
        }

    }
}
