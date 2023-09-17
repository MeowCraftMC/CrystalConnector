using System.Formats.Cbor;
using System.Net;
using System.Net.WebSockets;
using CrystalConnector.Configs;
using CrystalConnector.Connector.Packet.S2C;
using CrystalConnector.Handlers;
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
            var reader = new CborReader(new ReadOnlyMemory<byte>(buffer, 0, receiveResult.Count));

            try
            {
                var resultPacket = await Handler.Handle(webSocket, reader);
                await webSocket.Send(resultPacket);
            }
            catch (CborContentException ex)
            {
                if (Config.IsDebug())
                {
                    Logger.LogWarning(ex, "Unknown message!");
                }

                await webSocket.Send(new S2CUnknownPacket());
            }
            catch (InvalidOperationException ex)
            {
                if (Config.IsDebug())
                {
                    Logger.LogWarning(ex, "Malformed message!");
                }
                
                await webSocket.Send(new S2CMalformedPacket());
            }
            catch (MessageException ex)
            {
                Logger.LogWarning(ex, "Why do that?");
                await webSocket.Send(new S2CMalformedPacket());
            }
            
            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        webSocket.Purge();
    }

    public void Disconnect(WebSocket webSocket)
    {
        webSocket.Disconnect();
    }
}