namespace ProZapCommunication.Core.Models.StationData.Read;

public class StationReadBaseModel
{
    public int Life { get; set; }
    public int TaskCounter { get; set; }
    public int TaskSendToPc { get; set; }
    public int TaskConfirmToPc { get; set; }
    public int TaskSuccessCounter { get; set; }
    public int TaskStatus { get; set; }
    public int Camera_Status { get; set; }
    public string DMC { get; set; } = string.Empty;

}
