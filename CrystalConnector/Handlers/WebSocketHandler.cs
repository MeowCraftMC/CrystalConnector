using System.Formats.Cbor;
using System.Net.WebSockets;
using CrystalConnector.Configs;
using CrystalConnector.Connector.Packet;
using CrystalConnector.Utilities;
using Microsoft.Extensions.Logging;

namespace CrystalConnector.Handlers;

public class WebSocketHandler
{
    private ConfigAccessor Config { get; }

    private ILogger<WebSocketHandler> Logger { get; }
    
    public WebSocketHandler(ConfigAccessor config, ILogger<WebSocketHandler> logger)
    {
        Config = config;
        Logger = logger;
    }
    
    public async Task Handle(WebSocket webSocket, CborReader reader)
    {
        reader.ReadStartArray();
        
        var type = reader.ReadTextString();
        if (type == HandlerConstants.OperationAuthenticate)
        {
            if (!webSocket.GetConnectionInfo().Authenticated)
            {
                var secret = reader.ReadTextString();
                if (secret == Config.GetSecretKey())
                {
                    Logger.LogInformation("Client {Id} authorized", webSocket.GetConnectionInfo().Id);
                    webSocket.GetConnectionInfo().Authenticated = true;
                    await webSocket.Send(new S2CAuthenticatedPacket().Write);
                }
                else
                {
                    throw new MessageException("Auth failed!");
                }
            }
            else
            {
                throw new MessageException("You are already authorized.");
            }
        }
        else if (type == "")
        {
            
        }
        
        reader.ReadEndArray();
    }
}