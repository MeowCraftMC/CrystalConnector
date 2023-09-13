using System.Formats.Cbor;
using System.Net.WebSockets;
using CrystalConnector.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CrystalConnector.Handlers;

public class WebSocketHandler
{
    private IConfiguration Config { get; }

    private ILogger<WebSocketHandler> Logger { get; }
    
    public WebSocketHandler(IConfiguration config, ILogger<WebSocketHandler> logger)
    {
        Config = config;
        Logger = logger;
    }
    
    public void Handle(WebSocket webSocket, CborReader reader)
    {
        reader.ReadStartArray();
        
        var type = reader.ReadTextString();
        if (type == HandlerConstants.MessageTypeAuthenticate)
        {
            if (!webSocket.GetConnectionInfo().Authorized)
            {
                var secret = reader.ReadTextString();
                if (secret == Config.GetValue<string>("Connector:Auth:Secret"))
                {
                    Logger.LogInformation("Client {Id} authorized", webSocket.GetConnectionInfo().Id);
                    webSocket.GetConnectionInfo().Authorized = true;
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