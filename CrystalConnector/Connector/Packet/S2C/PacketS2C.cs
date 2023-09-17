using System.Formats.Cbor;

namespace CrystalConnector.Connector.Packet.S2C;

public abstract class PacketS2C : IPacket
{
    public void Write(CborWriter writer)
    {
        writer.WriteStartArray(null);
        WriteData(writer);
        writer.WriteEndArray();
    }

    protected abstract void WriteData(CborWriter writer);
}