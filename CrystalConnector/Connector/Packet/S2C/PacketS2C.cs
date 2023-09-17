using System.Formats.Cbor;

namespace CrystalConnector.Connector.Packet.S2C;

public abstract class PacketS2C : IPacket
{
    public void Write(CborWriter writer)
    {
        writer.WriteStartMap(null);
        WriteData(writer);
        writer.WriteEndMap();
    }

    protected abstract void WriteData(CborWriter writer);
}