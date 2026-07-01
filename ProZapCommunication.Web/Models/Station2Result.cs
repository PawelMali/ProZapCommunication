namespace ProZapCommunication.Web.Models;

public class Station2Result : StationResultBase
{
    public string Mfs_SN { get; set; } = string.Empty;
    public int Mfs_Status { get; set; }
    public float? Preasure { get; set; } // Zachowano oryginalną pisownię z SQL
    public int? TestTime { get; set; }
    public float? Leak { get; set; }
}
