namespace ProZapCommunication.Core.Models;

public class TaskHistoryEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Direction { get; set; } // "PLC -> PC" lub "PC -> PLC"
    public int Code { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public bool IsOk { get; set; }
}
