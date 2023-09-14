using System.Formats.Cbor;
using CrystalConnector.Handlers;

namespace CrystalConnector.Connector.Packet;

public class S2CAuthenticatedPacket : IPacket
{
    public void Write(CborWriter writer)
    {
        writer.WriteTextString(HandlerConstants.ResponseAuthenticated);
    }
}