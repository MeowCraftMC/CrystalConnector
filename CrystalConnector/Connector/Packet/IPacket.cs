using System.Formats.Cbor;

namespace CrystalConnector.Connector.Packet;

public interface IPacket
{
    public void Write(CborWriter writer)
    {
    }

    public void Read(CborReader reader)
    {
    }
}