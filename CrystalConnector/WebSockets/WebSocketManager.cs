using System.Formats.Cbor;
using System.Net;
using System.Net.WebSockets;
using CrystalConnector.Handlers;
using CrystalConnector.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrystalConnector.WebSockets;

public class WebSocketManager : IWebSocketManager
{
    private ILogger<WebSocketManager> Logger { get; }
    
    private IConfiguration Config { get; }

    private WebSocketHandler Handler { get; }

    public WebSocketManager(ILogger<WebSocketManager> logger, IConfiguration config, WebSocketHandler handler)
    {
        Logger = logger;
        Config = config;
        Handler = handler;
    }

    public async Task StartHandle(HttpContext context, WebSocket webSocket, TaskCompletionSource<WebSocketHandleResult> taskCompletionSource)
    {
        var remoteEndPoint = new IPEndPoint(context.Connection.RemoteIpAddress!, context.Connection.RemotePort);    // qyl27: we believe it is a tcp connection.
        webSocket.AddConnectionInfo(context.Connection.Id, remoteEndPoint, taskCompletionSource);
        await Receive(webSocket);
    }

    private async Task Receive(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            var reader = new CborReader(new ReadOnlyMemory<byte>(buffer, 0, receiveResult.Count));

            try
            {
                Handler.Handle(webSocket, reader);
            }
            catch (CborContentException ex)
            {
                if (Config.GetValue<bool>("Connector:Debug"))
                {
                    Logger.LogWarning(ex, "Malformed message!");
                }
                
                Disconnect(webSocket);
            }
            catch (InvalidOperationException ex)
            {
                if (Config.GetValue<bool>("Connector:Debug"))
                {
                    Logger.LogWarning(ex, "Bad message!");
                }
                Disconnect(webSocket);
            }
            catch (MessageException ex)
            {
                Logger.LogWarning(ex, "Why do that?");
                Disconnect(webSocket);
            }
            
            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }
    }

    public void Disconnect(WebSocket webSocket)
    {
        webSocket.Disconnect();
    }
}