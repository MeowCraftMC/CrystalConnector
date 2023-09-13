﻿using System.Net;
using System.Net.WebSockets;
using CrystalConnector.Connector;

namespace CrystalConnector.Utilities;

public static class WebSocketExtension
{
    public static void AddConnectionInfo(this WebSocket webSocket, string id, IPEndPoint ip, 
        TaskCompletionSource<WebSocketHandleResult> taskCompletionSource)
    {
        WebSocketConnectionManager.Info.Add(webSocket, new WebSocketConnectionInfo
        {
            Id = id, 
            IpEndPoint = ip,
            CompletionSource = taskCompletionSource,
        });
    }

    public static WebSocketConnectionInfo GetConnectionInfo(this WebSocket webSocket)
    {
        return WebSocketConnectionManager.Info[webSocket];
    }

    public static void Disconnect(this WebSocket webSocket)
    {
        var info = GetConnectionInfo(webSocket);
        info.CompletionSource.SetResult(WebSocketHandleResult.Successful);

        webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        
        WebSocketConnectionManager.Info.Remove(webSocket);
    }

    public static async Task Send(this WebSocket webSocket, byte[] data)
    {
        await webSocket.Send(data, 0, data.Length);
    }
    
    public static async Task Send(this WebSocket webSocket, byte[] data, int startIndex, int length)
    {
        await webSocket.SendAsync(new ReadOnlyMemory<byte>(data, startIndex, length), WebSocketMessageType.Binary, true, CancellationToken.None);
    }
}