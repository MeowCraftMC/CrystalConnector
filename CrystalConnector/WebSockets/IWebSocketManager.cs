using System.Net.WebSockets;
using CrystalConnector.Utilities;
using Microsoft.AspNetCore.Http;

namespace CrystalConnector.WebSockets;

public interface IWebSocketManager
{
    public Task StartHandle(HttpContext context, WebSocket socket, TaskCompletionSource<WebSocketHandleResult> taskCompletionSource);

    public void Disconnect(WebSocket socket);
}