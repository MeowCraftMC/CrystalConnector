using CrystalConnector.Protocol.Packets;
using Websocket.Client;

namespace CrystalConnector.Client.Extensions;

public static class WebsocketClientExtension
{
    public static void Send(this WebsocketClient client, IPacket packet)
    {
        client.Send(packet.Write());
    }
}