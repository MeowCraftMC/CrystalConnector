using System.Formats.Cbor;
using CrystalConnector.Handlers;

namespace CrystalConnector.Connector.Packet.S2C;

public class S2CForwardMessagePacket : PacketS2C
{
    private string Origin { get; }
    private string ChannelId { get; }
    private byte[] Payload { get; }
    
    public S2CForwardMessagePacket(string origin, string channelId, byte[] payload)
    {
        Origin = origin;
        ChannelId = channelId;
        Payload = payload;
    }
    
    protected override void WriteData(CborWriter writer)
    {
        writer.WriteTextString(HandlerConstants.ResponseForward);
        writer.WriteTextString(Origin);
        writer.WriteTextString(ChannelId);
        writer.WriteByteString(Payload);
    }
}