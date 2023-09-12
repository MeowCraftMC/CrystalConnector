using System.Net;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CrystalConnector.Handlers;

public class WebSocketHandler : IWebSocketHandler
{
    private ILogger<WebSocketHandler> Logger { get; }

    // Todo: qyl27: Maybe we need a MultiKeyMap? <string Id, IPEndPoint ip, Tuple<WebSocket, TaskCompletionSource<WebSocketHandleResult>>>
    private Dictionary<IPEndPoint, Tuple<WebSocket, TaskCompletionSource<WebSocketHandleResult>>> WebSockets { get; } = new();

    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        Logger = logger;
    }

    public void StartHandle(HttpContext context, WebSocket socket, TaskCompletionSource<WebSocketHandleResult> taskCompletionSource)
    {
        var remoteEndPoint = new IPEndPoint(context.Connection.RemoteIpAddress, context.Connection.RemotePort);
        WebSockets.Add(remoteEndPoint, new Tuple<WebSocket, TaskCompletionSource<WebSocketHandleResult>>(socket, taskCompletionSource));
    }

    public void Disconnect(WebSocket socket)
    {
        
    }
}