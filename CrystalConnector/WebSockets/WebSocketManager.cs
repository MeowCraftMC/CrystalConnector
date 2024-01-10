using System.Net;
using System.Net.WebSockets;
using CrystalConnector.Configs;
using CrystalConnector.Handlers;
using CrystalConnector.Protocol.Messages;
using CrystalConnector.Protocol.Packets.S2C;
using CrystalConnector.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CrystalConnector.WebSockets;

public class WebSocketManager : IWebSocketManager
{
    private ILogger<WebSocketManager> Logger { get; }
    
    private ConfigAccessor Config { get; }

    private WebSocketHandler Handler { get; }

    public WebSocketManager(ILogger<WebSocketManager> logger, ConfigAccessor config, WebSocketHandler handler)
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
            var bytes = new ReadOnlyMemory<byte>(buffer, 0, receiveResult.Count);

            try
            {
                var resultPacket = await Handler.Handle(webSocket, bytes.ToArray());
                await webSocket.Send(resultPacket);
            }
            catch (Exception ex)
            {
                if (Config.IsDebug())
                {
                    Logger.LogWarning(ex, "Malformed message!");
                }
                
                await webSocket.Send(new ResultPacket(Error.Internal));
            }

            try
            {
                receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.LogInformation("Client {Name}({Id}) disconnected!", 
                    webSocket.GetConnectionInfo().Name, webSocket.GetConnectionInfo().Id);
                webSocket.Purge();
                break;
            }
        }
    }

    public void Disconnect(WebSocket webSocket)
    {
        webSocket.Disconnect();
    }
}