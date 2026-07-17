namespace ProZapCommunication.Core.Models.StationData.Write;

public class StationWriteBaseModel
{
    public int Life { get; set; }
    public int TaskCounter { get; set; }
    public int TaskSendToPLC { get; set; }
    public int TaskConfirmToPLC { get; set; }
    public int TaskSuccessCounter { get; set; }
    public int TaskStatus { get; set; }
    public int CameraFound { get; set; }
    public int CameraMoved { get; set; }
}
