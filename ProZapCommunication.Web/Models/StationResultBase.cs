namespace ProZapCommunication.Web.Models;

public abstract class StationResultBase
{
    public long Id { get; set; }
    public int StationId { get; set; }
    public DateTime Timestamp { get; set; }
    public string SN { get; set; } = string.Empty;
    public int SN_Status { get; set; }
    public int CycleTime { get; set; }
}
