﻿using System.Net;
using CrystalConnector.Connector;

namespace CrystalConnector.Utilities;

public class WebSocketConnectionInfo
{
    public required string Id { get; set; }
    
    public required IPEndPoint IpEndPoint { get; set; }
    
    public required TaskCompletionSource<WebSocketHandleResult> CompletionSource { get; set; }

    public string? Name { get; set; }
    
    public (string Namespace, string Name)? ClientId { get; set; }
    
    public bool Authenticated { get; set; } = false;

    public Dictionary<(string Namespace, string Name), MessageChannel> RegisteredChannels { get; } = new();
}