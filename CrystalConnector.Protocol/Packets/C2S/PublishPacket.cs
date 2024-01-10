using CrystalConnector.Protocol.Messages;
using Google.Protobuf;

namespace CrystalConnector.Protocol.Packets.C2S;

public class PublishPacket : IPacket
{
    public NamespacedName Channel { get; set; }
    public string Payload { get; set; }
    
    public PublishPacket(NamespacedName channel, string payload)
    {
        Channel = channel;
        Payload = payload;
    }
    
    public PublishPacket(byte[] bytes)
    {
        Read(bytes);
    }

    public PublishPacket()
    {
    }
    
    public void Read(byte[] bytes)
    {
        var message = Publish.Parser.ParseFrom(bytes);
        Channel = message.Channel;
        Payload = message.Payload;
    }

    public byte[] Write()
    {
        var message = new Publish
        {
            Channel = Channel,
            Payload = Payload
        };

        return message.ToByteArray();
    }
}