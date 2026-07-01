namespace ProZapCommunication.Core.Models.Config;

public class AppConfig
{
    public string ConnectionString { get; set; }
    public List<PlcConfig> PlcDevices { get; set; } = new List<PlcConfig>();
}

public class PlcConfig
{
    public string Name { get; set; }
    public string IpAddress { get; set; }
    public string CpuPath { get; set; } = "1,0";
    public List<StationConfig> Stations { get; set; } = new List<StationConfig>();
}

public class StationConfig
{
    public int StationId { get; set; }
    public string Name { get; set; }
    public string StationType { get; set; } // np. "Screwing", "LeakTest"
    public List<TagConfig> Tags { get; set; } = new List<TagConfig>();
}

public class TagConfig
{
    public string Purpose { get; set; } // np. "HandshakeRead", "HandshakeWrite", "ProcessData"
    public string TagName { get; set; } // np. "St1_HandshakeRead"
}
