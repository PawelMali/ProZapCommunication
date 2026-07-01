namespace ProZapCommunication.Web.Models;

public class Station4Result : StationResultBase
{
    public float? Temperature { get; set; }
    public int? GlueTupe { get; set; } // Zachowano oryginalną pisownię z SQL
    public float? Weight { get; set; }
}
