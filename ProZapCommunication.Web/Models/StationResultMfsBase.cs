namespace ProZapCommunication.Web.Models;

public abstract class StationResultMfsBase
{
    public long Id { get; set; }
    public int StationId { get; set; }
    public DateTime Timestamp { get; set; }
    public string Motor_SN { get; set; } = string.Empty; // Główny identyfikator na stacjach 5-7 MFS
    public int Motor_Status { get; set; }
    public int CycleTime { get; set; }
    public int Reference_Number { get; set; }
}
