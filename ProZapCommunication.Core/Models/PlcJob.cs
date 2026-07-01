namespace ProZapCommunication.Core.Models;

public class PlcJob
{
    public string Name { get; }
    public Action Execute { get; }
    public bool IsActive { get; set; } = true;

    public PlcJob(string name, Action execute)
    {
        Name = name;
        Execute = execute;
    }
}
