using System.Formats.Cbor;
using CrystalConnector.Handlers;

namespace CrystalConnector.Connector.Packet.S2C;

public class S2CUndefinedDirectionPacket : PacketS2C
{
    protected override void WriteData(CborWriter writer)
    {
        writer.WriteTextString(HandlerConstants.ResponseErrorUndefinedDirection);
    }
}