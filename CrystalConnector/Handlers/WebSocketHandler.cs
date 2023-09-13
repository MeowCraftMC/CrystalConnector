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
        var type = reader.ReadTextString();
        if (type == "Authenticate")
        {
            if (!webSocket.GetConnectionInfo().Authorized)
            {
                var secret = reader.ReadTextString();
                if (secret == Config.GetValue<string>("Connector:Auth:Secret"))
                {
                    webSocket.GetConnectionInfo().Authorized = true;
                }
            }
        }
    }
}