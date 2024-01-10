using CrystalConnector.Protocol.Entities;
using CrystalConnector.Protocol.Messages;

namespace CrystalConnector.Connector;

public class MessageChannel
{
    public required NamespacedId Id { get; set; }
    
    public required MessageDirection Direction { get; set; }
}