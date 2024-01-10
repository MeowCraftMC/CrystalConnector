using CrystalConnector.Protocol.Messages;

namespace CrystalConnector.Connector;

public class MessageChannel
{
    public required (string Namespace, string Name) Id { get; set; }
    
    public required MessageDirection Direction { get; set; }
}