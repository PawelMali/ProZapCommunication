using AutomatedSolutions.ASCommStd;
using AutomatedSolutions.ASCommStd.AB.Logix.Data;
using ProZapCommunication.Core.Interfaces;
using ProZapCommunication.Core.Models.StationData.Read;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;
using ProZapCommunication.Infrastructure.Plc.Logix.Mappers;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Read;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Write;
using Serilog;
using System.Collections.Concurrent;
using ABLogix = AutomatedSolutions.ASCommStd.AB.Logix;

namespace ProZapCommunication.Infrastructure.Plc.Logix;

public class LogixAsCommPlcProvider : IPlcProvider
{
    private readonly string _ipAddress;

    // Główne komponenty sterownika ASComm
    private readonly ABLogix.Net.Channel _myChannel;
    private readonly ABLogix.Device _myDevice;
    private readonly ABLogix.Group _myGroup;

    // Słownik keszujący obiekty Item dla optymalizacji pamięci i wydajności PLC
    private readonly ConcurrentDictionary<string, ABLogix.Item> _itemsCache = new ConcurrentDictionary<string, ABLogix.Item>();

    // Status połączenia – aktualizowany dynamicznie przy próbach odczytu/zapisu
    public bool IsConnected { get; private set; }
    // Flag pamiętająca poprzedni stan połączenia (do filtrowania logów)
    private bool _wasConnectedLastTime = true;

    // Dedykowany logger dla tej konkretnej instancji PLC, który automatycznie 
    // dorzuca do każdej ramki logu informację o swoim IP (czyli nasz klucz PlcIdentity)
    private readonly ILogger _plcLogger;

    public LogixAsCommPlcProvider(string ipAddress, string cpuPath = "1,0")
    {
        _ipAddress = ipAddress;

        // Przygotowanie bezpiecznego dla nazw plików identyfikatora podmiotu (np. "192_168_0_10")
        string plcKey = _ipAddress.Replace(".", "_");
        _plcLogger = Log.ForContext("PlcIdentity", plcKey);

        try
        {
            _plcLogger.Debug("Initialization of ASComm objects for the driver.");

            // 1. Tworzenie kanału (zgodnie z przykładem, wygeneruje wyjątek licencji jeśli trial wygasł)
            _myChannel = new ABLogix.Net.Channel();

            // 2. Tworzenie urządzenia (Model: ControlLogix, ścieżka np. "192.168.0.18,1,0")
            _myDevice = new ABLogix.Device($"{_ipAddress},{cpuPath}", ABLogix.Model.ControlLogix);

            // 3. Tworzenie grupy z wyłączonym próbkowaniem automatycznym (Active = false)
            // Będziemy jawnie i cyklicznie wywoływać metodę .Read() w harmonogramie
            _myGroup = new ABLogix.Group(false, 500);

            // 4. Budowanie hierarchii obiektów ASComm
            _myChannel.Devices.Add(_myDevice);
            _myDevice.Groups.Add(_myGroup);

            // Zakładamy wstępnie true, faktyczny stan zweryfikuje pierwszy odczyt
            IsConnected = true;
        }
        catch (Exception ex)
        {
            IsConnected = false;
            _plcLogger.Error(ex, "CRITICAL ERROR: Initialization of communication objects failed.");
            throw;
        }
    }

    public void ReadStructure(string tagName, object plcStructureTarget)
    {
        try
        {
            // Pobierz istniejący obiekt Item lub utwórz go przy pierwszym zapytaniu
            ABLogix.Item item = GetOrAddItem(tagName);

            // Jawne wywołanie odczytu synchronicznego. 
            // Tutaj aplikacja dowie się o braku fizycznego połączenia lub błędzie tagu.
            item.Read();

            // Przepisanie odebranych bajtów na pola przekazanej klasy struktury
            item.GetStructuredValues(plcStructureTarget);

            // Jeśli odczyt przeszedł pomyślnie, połączenie jest sprawne
            IsConnected = true;

            // --- OBSŁUGA POWROTU POŁĄCZENIA ---
            if (!_wasConnectedLastTime)
            {
                _plcLogger.Information("CONNECTION RESTORED for PLC ({IP}). Resuming normal data exchange.", _ipAddress);
                _wasConnectedLastTime = true;
            }
        }
        catch (Exception ex)
        {
            IsConnected = false;

            // Analiza komunikatu błędu pod kątem błędów konfiguracji tagów (CIP 0x00000104 / IOI syntax)
            string errorMsg = ex.Message.ToLower();

        // TYP 1: BŁĄD ROZMIARU STRUKTURY (Data Length Mismatch)
        if (errorMsg.Contains("does not match computed data length") || errorMsg.Contains("source data length"))
        {
            _plcLogger.Error("CRITICAL STRUCTURE SIZE ERROR! The structure of tag '{TagName}' in the PLC has a different size than the C# class. Polling will be disabled. Message: {Msg}", 
                tagName, ex.Message);
            
            throw new ProZapCommunication.Core.Exceptions.PlcStructureMismatchException(ex.Message);
        }

        // TYP 2: CAŁKOWITY BRAK TAGA W PLC
        if (errorMsg.Contains("ioi syntax") || errorMsg.Contains("does not exist") || errorMsg.Contains("0x00000104"))
        {
            _plcLogger.Error("CRITICAL ADDRESSING ERROR! Tag '{TagName}' does not exist in the PLC. Polling will be disabled.", tagName);
            
            throw new ProZapCommunication.Core.Exceptions.PlcMissingTagException(ex.Message);
        }

        // TYP 3: Standardowa utrata połączenia (odpięty kabel, timeout sieciowy)
        if (_wasConnectedLastTime)
        {
            _plcLogger.Error("CONNECTION LOST with PLC ({IP})! First error on tag '{TagName}'.", _ipAddress, tagName);
            _wasConnectedLastTime = false; 
        }

            throw;
        }
    }

    public void WriteStructure(string tagName, object plcStructureSource)
    {
        try
        {
            ABLogix.Item item = GetOrAddItem(tagName);

            int size = plcStructureSource is null ? 0 : System.Runtime.InteropServices.Marshal.SizeOf(plcStructureSource);

            // Bezpośredni zapis struktury (obiektu WRITE) do sterownika
            item.Write(plcStructureSource);

            IsConnected = true;

            if (!_wasConnectedLastTime)
            {
                _plcLogger.Information("CONNECTION RESTORED (Write) for PLC ({IP}).", _ipAddress);
                _wasConnectedLastTime = true;
            }
        }
        catch (Exception ex)
        {
            IsConnected = false;
            string errorMsg = ex.Message.ToLower();

            // TYP 1: BŁĄD ROZMIARU STRUKTURY PRZY ZAPISIE (Not enough data / CIP 0x00000013)
            if (errorMsg.Contains("not enough data") || errorMsg.Contains("0x00000013") || errorMsg.Contains("data length"))
            {
                _plcLogger.Error("CRITICAL STRUCTURE SIZE ERROR! The WRITE structure for tag '{TagName}' has a different size than the PLC memory. " +
                                 "Write operation blocked. Message: {Msg}", tagName, ex.Message);

                // Rzucamy ten sam dedykowany wyjątek Core, co przy odczycie
                throw new ProZapCommunication.Core.Exceptions.PlcStructureMismatchException(ex.Message);
            }

            // TYP 2: CAŁKOWITY BRAK TAGA DO ZAPISU W PLC (CIP 0x00000104 / IOI syntax)
            if (errorMsg.Contains("ioi syntax") || errorMsg.Contains("does not exist") || errorMsg.Contains("0x00000104"))
            {
                _plcLogger.Error("CRITICAL ADDRESSING ERROR! Tag '{TagName}' does not exist in the PLC. Write operation blocked.", tagName);

                throw new ProZapCommunication.Core.Exceptions.PlcMissingTagException(ex.Message);
            }

            // TYP 3: Standardowa utrata połączenia (odpięty kabel, timeout sieciowy)
            if (_wasConnectedLastTime)
            {
                _plcLogger.Error("CONNECTION LOST with PLC ({IP}) during write operation! First error on tag '{TagName}'.", _ipAddress, tagName);
                _wasConnectedLastTime = false;
            }

            throw;
        }
    }

    public void Disconnect()
    {
        try
        {
            _plcLogger.Information("DISCONNECTING PLC and clearing cache.");
            _itemsCache.Clear();
            _myChannel?.Dispose();
            IsConnected = false;
        }
        catch (Exception ex)
        {
            _plcLogger.Error(ex, "ERROR occurred while releasing communication channel resources.");
        }
    }

    /// <summary>
    /// Zapewnia, że dla każdego taga w sterowniku powstanie tylko jedna instancja obiektu Item.
    /// </summary>
    private ABLogix.Item GetOrAddItem(string tagName)
    {
        return _itemsCache.GetOrAdd(tagName, targetTag =>
        {
            _plcLogger.Debug("Registering a new tag in the RAM drivers: '{TagName}'", targetTag);

            var newItem = new ABLogix.Item($"item_{targetTag}", targetTag);
            _myGroup.Items.Add(newItem);

            return newItem;
        });
    }


    public StationReadBaseModel ReadBase(string tagName)
    {
        // 1. Tworzymy surową strukturę Logix
        var rawData = new LogixStationReadBase();

        // 2. Czytamy z PLC
        ReadStructure(tagName, rawData);

        // 3. Zwracamy zmapowany, czysty model do Core
        return rawData.ToCoreModel();
    }

    public Station8ReadBaseModel ReadStation8Base(string tagName)
    {
        // 1. Tworzymy surową strukturę Logix
        var rawData = new LogixStationReadBase();

        // 2. Czytamy z PLC
        ReadStructure(tagName, rawData);

        // 3. Zwracamy zmapowany, czysty model do Core
        return rawData.ToCoreModel8();
    }

    public void WriteBase(string tagName, StationWriteBaseModel data)
    {
        // 1. Mapujemy czysty model Core na strukturę zapisu Logix
        LogixStationWriteBase rawData = data.ToPlcStructure();

        // 2. Zapisujemy do PLC
        var item = GetOrAddItem(tagName);
        item.Write(rawData);
    }

    public void WriteStation7Base(string tagName, Station7WriteBaseModel data)
    {
        // 1. Mapujemy czysty model Core na strukturę zapisu Logix
        LogixStationWriteBase rawData = data.ToPlcStructure();

        // 2. Zapisujemy do PLC
        var item = GetOrAddItem(tagName);
        item.Write(rawData);
    }

    public Station1ResultsData ReadStation1ResultsData(string tagName)
    {
        var rawData = new LogixStation1ResultsData();
        ReadStructure(tagName, rawData);
        return rawData.ToCoreModel();
    }

    public Station2ResultsData ReadStation2ResultsData(string tagName)
    {
        var rawData = new LogixStation2ResultsData();
        ReadStructure(tagName, rawData);
        return rawData.ToCoreModel();
    }

    public Station3ResultsData ReadStation3ResultsData(string tagName)
    {
        var rawData = new LogixStation3ResultsData();
        ReadStructure(tagName, rawData);
        return rawData.ToCoreModel();
    }

    public Station4ResultsData ReadStation4ResultsData(string tagName)
    {
        var rawData = new LogixStation4ResultsData();
        ReadStructure(tagName, rawData);
        return rawData.ToCoreModel();
    }

    public Station5ResultsData ReadStation5ResultsData(string tagName)
    {
        var rawData = new LogixStation5ResultsData();
        ReadStructure(tagName, rawData);
        return rawData.ToCoreModel();
    }

    public Station6ResultsData ReadStation6ResultsData(string tagName)
    {
        var rawData = new LogixStation6ResultsData();
        ReadStructure(tagName, rawData);
        return rawData.ToCoreModel();
    }

    public Station7ResultsData ReadStation7ResultsData(string tagName)
    {
        var rawData = new LogixStation7ResultsData();
        ReadStructure(tagName, rawData);
        return rawData.ToCoreModel();
    }
}