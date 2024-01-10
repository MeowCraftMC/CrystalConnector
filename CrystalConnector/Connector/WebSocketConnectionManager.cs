using System.Net.WebSockets;
using CrystalConnector.Protocol.Entities;
using CrystalConnector.Protocol.Messages;
using CrystalConnector.Protocol.Packets.S2C;
using CrystalConnector.Utilities;

namespace CrystalConnector.Connector;

public class WebSocketConnectionManager
{
    internal static Dictionary<WebSocket, WebSocketConnectionInfo> Info { get; } = new();

    public static bool HaveClientRegistered(NamespacedId client)
    {
        foreach (var (_, info) in Info)
        {
            if (info.ClientId == client)
            {
                return true;
            }
        }
        
        return false;
    }

    public static void DisconnectAll()
    {
        foreach (var (webSocket, _) in Info)
        {
            webSocket.Disconnect();
        }
    }

    public static async Task Broadcast(string origin, NamespacedId channel, string payload, bool allowUnauthenticated = false)
    {
        var packet = new ForwardPacket(origin, channel, payload);
        
        foreach (var (webSocket, info) in Info)
        {
            if (info.RegisteredChannels.TryGetValue(channel, out var value) 
                && value.Direction is MessageDirection.Incoming or MessageDirection.All)
            {
                if (webSocket.State != WebSocketState.Open)
                {
                    webSocket.Purge();
                    continue;
                }
                
                await webSocket.Send(packet.Write());
            }
        }
    }
}