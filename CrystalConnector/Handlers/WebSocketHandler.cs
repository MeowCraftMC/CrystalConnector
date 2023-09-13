using System.Net;
using System.Net.WebSockets;
using CrystalConnector.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CrystalConnector.Handlers;

public class WebSocketHandler : IWebSocketHandler
{
    private ILogger<WebSocketHandler> Logger { get; }

    public WebSocketHandler(ILogger<WebSocketHandler> logger)
    {
        Logger = logger;
    }

    public async void StartHandle(HttpContext context, WebSocket webSocket, TaskCompletionSource<WebSocketHandleResult> taskCompletionSource)
    {
        var remoteEndPoint = new IPEndPoint(context.Connection.RemoteIpAddress!, context.Connection.RemotePort);    // qyl27: we believe it is a tcp connection.
        var receiveCancellationTokenSource = new CancellationTokenSource();
        webSocket.AddData(context.Connection.Id, remoteEndPoint, taskCompletionSource, receiveCancellationTokenSource);
        await Handle(webSocket, receiveCancellationTokenSource.Token);
    }

    private async Task Handle(WebSocket webSocket, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        
        WebSocketReceiveResult receiveResult;
        
        do
        {
            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
        } while (!cancellationToken.IsCancellationRequested && !receiveResult.CloseStatus.HasValue);
    }

    public void Disconnect(WebSocket webSocket)
    {
        webSocket.RemoveData();
    }
}