using System.Net.WebSockets;
using CrystalConnector.Utilities;
using Microsoft.AspNetCore.Http;

namespace CrystalConnector.Handlers;

public interface IWebSocketHandler
{
    public void StartHandle(HttpContext context, WebSocket socket, TaskCompletionSource<WebSocketHandleResult> taskCompletionSource);

    public void Disconnect(WebSocket socket);
}