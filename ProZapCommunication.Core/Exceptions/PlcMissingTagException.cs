namespace ProZapCommunication.Core.Exceptions;

public class PlcMissingTagException : Exception
{
    public PlcMissingTagException(string message) : base(message) { }
}
