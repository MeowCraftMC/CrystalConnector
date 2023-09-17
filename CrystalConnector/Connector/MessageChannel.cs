namespace CrystalConnector.Connector;

public class MessageChannel
{
    public required string Id { get; set; }
    
    public required MessageDirection Direction { get; set; }
}