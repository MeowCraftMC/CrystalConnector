using System.Net;
using CrystalConnector.Connector;

namespace CrystalConnector.Utilities;

public class WebSocketConnectionInfo
{
    public required string Id { get; set; }
    
    public required IPEndPoint IpEndPoint { get; set; }
    
    public required TaskCompletionSource<WebSocketHandleResult> CompletionSource { get; set; }

    public bool Authorized { get; set; } = false;

    public List<MessageChannel> RegisteredChannels { get; } = new();
}