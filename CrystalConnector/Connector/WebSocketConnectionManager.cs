using System.Net.WebSockets;
using CrystalConnector.Utilities;

namespace CrystalConnector.Connector;

public class WebSocketConnectionManager
{
    internal static Dictionary<WebSocket, WebSocketConnectionInfo> Info { get; } = new();

    public static void DisconnectAll()
    {
        foreach (var (webSocket, info) in Info)
        {
            webSocket.Disconnect();
        }
    }
}