using System.Net;
using CrystalConnector.Connector;
using CrystalConnector.Protocol.Entities;

namespace CrystalConnector.Utilities;

public class WebSocketConnectionInfo
{
    public required string Id { get; set; }
    
    public required IPEndPoint IpEndPoint { get; set; }
    
    public required TaskCompletionSource<WebSocketHandleResult> CompletionSource { get; set; }

    public string? Name { get; set; }
    
    public NamespacedId? ClientId { get; set; }
    
    public bool Authenticated { get; set; } = false;

    public Dictionary<NamespacedId, MessageChannel> RegisteredChannels { get; } = new();
}