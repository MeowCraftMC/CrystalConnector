using System.Net.WebSockets;
using CrystalConnector.Connector.Packet.S2C;
using CrystalConnector.Utilities;

namespace CrystalConnector.Connector;

public class WebSocketConnectionManager
{
    internal static Dictionary<WebSocket, WebSocketConnectionInfo> Info { get; } = new();

    public static bool HaveNameRegistered(string name)
    {
        foreach (var (webSocket, info) in Info)
        {
            if (info.Name == name)
            {
                return true;
            }
        }
        
        return false;
    }

    public static void DisconnectAll()
    {
        foreach (var (webSocket, info) in Info)
        {
            webSocket.Disconnect();
        }
    }

    public static async Task Broadcast(string origin, string channelId, byte[] payload, bool allowUnauthenticated = false)
    {
        var packet = new S2CForwardMessagePacket(origin, channelId, payload);
        
        foreach (var (webSocket, info) in Info)
        {
            if (info.RegisteredChannels.TryGetValue(channelId, out var value) 
                && value.Direction.HasFlag(MessageDirection.Incoming) 
                && info.Name != origin)
            {
                if (webSocket.State != WebSocketState.Open)
                {
                    webSocket.Purge();
                    continue;
                }
                
                await webSocket.Send(packet);
            }
        }
    }
}