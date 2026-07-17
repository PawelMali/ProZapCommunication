using Microsoft.Extensions.Configuration;
using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Logic;
using ProZapCommunication.Core.Logic.StationLogics;
using ProZapCommunication.Core.Models;
using ProZapCommunication.Core.Models.Config;
using ProZapCommunication.Core.Models.StationData;
using ProZapCommunication.Core.Models.StationData.Write;
using ProZapCommunication.Infrastructure.Database;
using ProZapCommunication.Infrastructure.Plc.Logix;
using ProZapCommunication.Infrastructure.Plc.Logix.Mappers;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData;
using ProZapCommunication.UI.ViewModels;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProZapCommunication.UI;

public class Bootstrapper
{
    private readonly List<StationScheduler> _schedulers = new List<StationScheduler>();
    private readonly List<StationProcessorBase> _processors = new List<StationProcessorBase>();
    private readonly List<IPlcProvider> _plcProviders = new List<IPlcProvider>();

    public MainViewModel MainVm { get; private set; }

    private IDatabaseRepository _dbRepository;



    public void InitializeAndStart()
    {
        Log.Information("Initializing ProZapCommunication communication engine...");
        Log.Information("Loading configuration file from config/...");

        try
        {
            // 1. Budowanie managera konfiguracji dla ścieżki config/appsettings.json
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config"))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configRoot = configBuilder.Build();

            // Automatyczne mapowanie sekcji JSON na naszą strukturę klas C#
            AppConfig appConfig = configRoot.Get<AppConfig>();

            // 2. Inicjalizacja wspólnego repozytorium bazy danych
            var dbRepository = new SqlRepository(appConfig.ConnectionString);

            // Tworzymy główny ViewModel UI (przypiszemy mu pierwszy uniwersalny provider dla statusu ogólnego)
            MainVm = new MainViewModel();

            // Pętla główna – przechodzi po sterownikach z pliku JSON
            foreach (PlcConfig plcCfg in appConfig.PlcDevices)
            {
                IPlcProvider plcProvider = new LogixAsCommPlcProvider(plcCfg.IpAddress, plcCfg.CpuPath);
                _plcProviders.Add(plcProvider);
                MainVm.RegisterPlcProvider(plcProvider);

                // Przechodzimy po stacjach z konfiguracji i JAWNIE delegujemy ich tworzenie
                foreach (StationConfig stationCfg in plcCfg.Stations)
                {
                    // Używamy prostego switcha TYLKO do wyboru dedykowanej metody
                    switch (stationCfg.StationType.ToLower())
                    {
                        case "station1":
                            ConfigureStation1(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;
                        case "station2":
                            ConfigureStation2(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;
                        case "station3":
                            ConfigureStation3(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;
                        case "station4":
                            ConfigureStation4(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;
                        case "station5":
                            ConfigureStation5(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;
                        case "station6":
                            ConfigureStation6(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;
                        case "station7":
                            ConfigureStation7(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;
                        case "rework":
                            ConfigureStation8(plcCfg, stationCfg, plcProvider, dbRepository);
                            break;

                        default:
                            Log.Error("Unknown station type in configuration file: {Type}. Skipping.", stationCfg.StationType);
                            break;
                    }
                }
            }

            // 5. Uruchomienie wszystkich wątków harmonogramu
            foreach (var scheduler in _schedulers) scheduler.Start();
            Log.Information("All defined logical stations have been successfully initialized.");
        
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "APPLICATION INITIALIZATION ERROR! Check the correctness of the config/appsettings.json file.");
            throw;
        }

        Log.Information("All threads and communication levels have been successfully started.");
    }

    private void ConfigureStation1(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station1Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station1ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadBase(tagHandshakeRead);

                scheduler.FeedWatchdog(freshRead.Life);
                // Przetworzenie maszyny stanów w RAM
                processor.UpdateHandshakeData(freshRead);

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteBase(tagHandshakeWrite, processor.GetWriteState());
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    private void ConfigureStation2(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station2Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station2ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadBase(tagHandshakeRead);

                scheduler.FeedWatchdog(freshRead.Life);
                // Przetworzenie maszyny stanów w RAM
                processor.UpdateHandshakeData(freshRead);

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteBase(tagHandshakeWrite, processor.GetWriteState());
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    private void ConfigureStation3(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station3Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station3ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadBase(tagHandshakeRead);

                scheduler.FeedWatchdog(freshRead.Life);
                // Przetworzenie maszyny stanów w RAM
                processor.UpdateHandshakeData(freshRead);

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteBase(tagHandshakeWrite, processor.GetWriteState());
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    private void ConfigureStation4(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station4Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station4ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadBase(tagHandshakeRead);

                scheduler.FeedWatchdog(freshRead.Life);
                // Przetworzenie maszyny stanów w RAM
                processor.UpdateHandshakeData(freshRead);

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteBase(tagHandshakeWrite, processor.GetWriteState());
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    private void ConfigureStation5(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station5Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station5ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadBase(tagHandshakeRead);

                scheduler.FeedWatchdog(freshRead.Life);
                // Przetworzenie maszyny stanów w RAM
                processor.UpdateHandshakeData(freshRead);

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteBase(tagHandshakeWrite, processor.GetWriteState());
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    private void ConfigureStation6(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station6Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station6ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadBase(tagHandshakeRead);

                scheduler.FeedWatchdog(freshRead.Life);
                // Przetworzenie maszyny stanów w RAM
                processor.UpdateHandshakeData(freshRead);

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteBase(tagHandshakeWrite, processor.GetWriteState());
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    private void ConfigureStation7(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station7Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station7ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadBase(tagHandshakeRead);

                // Przetworzenie maszyny stanów w RAM
                scheduler.FeedWatchdog(freshRead.Life);
                processor.UpdateHandshakeData(freshRead);
                

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteStation7Base(tagHandshakeWrite, (Station7WriteBaseModel)processor.GetWriteState());

                processor.WriteBaseMenagment();
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    private void ConfigureStation8(PlcConfig plcCfg, StationConfig cfg, IPlcProvider plc, IDatabaseRepository db)
    {
        Log.Information("Dedicated initialization: Station {Id} ({Name})", cfg.StationId, cfg.Name);

        string stationIdentity = GenetateStationIdentity(cfg.Name);

        // Wyciągamy nazwy tagów unikalne dla stacji wkręcania
        string tagHandshakeRead = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeRead")?.TagName;
        string tagHandshakeWrite = cfg.Tags.FirstOrDefault(t => t.Purpose == "HandshakeWrite")?.TagName;
        string tagResults = cfg.Tags.FirstOrDefault(t => t.Purpose == "Results")?.TagName;

        var scheduler = new StationScheduler(cfg.StationId, plc);
        var processor = new Station8Processor(cfg.StationId, db, plc, stationIdentity, tagResults);
        var viewModel = new Station8ViewModel(cfg.StationId, cfg.Name, scheduler, processor);

        // Konfiguracja systemu wizyjnego, jeśli ścieżki są podane w konfiguracji
        if (!string.IsNullOrWhiteSpace(cfg.VisionSourcePath) && !string.IsNullOrWhiteSpace(cfg.VisionDestinationPath))
        {
            // To wywołanie zasili ścieżkę w procesorze, z której pobierzemy literę dysku w Task 9
            processor.ConfigureVisionSystem(cfg.VisionSourcePath, cfg.VisionDestinationPath);
        }

        // STOPIEŃ 1 (Szybka pętla cykliczna 50ms): Jeden czysty, funkcyjny blok odczytu i zapisu!
        scheduler.Level1FastJobs.Add(new PlcJob("Szybki_Handshake", () =>
        {
            using (Serilog.Context.LogContext.PushProperty("StationIdentity", stationIdentity))
            {
                // Odczyt struktury READ z interfejsu PLC
                var freshRead = plc.ReadStation8Base(tagHandshakeRead);

                // Przetworzenie maszyny stanów w RAM
                scheduler.FeedWatchdog(freshRead.Life);
                processor.UpdateHandshakeData(freshRead);

                // Natychmiastowe odesłanie struktury WRITE (W tym echa sygnału Life)
                plc.WriteBase(tagHandshakeWrite, processor.GetWriteState());
            }
        }));

        // Rejestracja obiektów w systemie
        _processors.Add(processor);
        _schedulers.Add(scheduler);
        MainVm.Stations.Add(viewModel);
    }

    public void ShutDown()
    {
        Log.Information("Shutting down communication engine...");
        foreach (var scheduler in _schedulers) scheduler.Stop();
        foreach (var provider in _plcProviders) provider.Disconnect();
    }

    public string GenetateStationIdentity(string name)
    {
        return name
        .Replace(" ", "_")
        .Replace("-", "_")
        .Replace("Ł", "L").Replace("ł", "l")
        .Replace("Ę", "E").Replace("ę", "e")
        .Replace("Ó", "O").Replace("ó", "o")
        .Replace("Ą", "A").Replace("ą", "a")
        .Replace("Ś", "S").Replace("ś", "s")
        .Replace("Ż", "Z").Replace("ż", "z")
        .Replace("Ź", "Z").Replace("ź", "z")
        .Replace("Ć", "C").Replace("ć", "c")
        .Replace("Ń", "N").Replace("ń", "n");
    }


}
