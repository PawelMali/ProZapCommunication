namespace ProZapCommunication.Web.Models;

public class Station1Result : StationResultBase
{
    public string Radiator_SN { get; set; } = string.Empty;
    public int Radiator_Status { get; set; }
    public float? Torque { get; set; }
    public float? Angle { get; set; }
    public int? Screw { get; set; }
}
