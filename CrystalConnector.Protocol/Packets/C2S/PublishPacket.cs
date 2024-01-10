using CrystalConnector.Protocol.Entities;
using CrystalConnector.Protocol.Messages;
using CrystalConnector.Protocol.Utilities;
using Google.Protobuf;

namespace CrystalConnector.Protocol.Packets.C2S;

public class PublishPacket : IPacket
{
    public NamespacedId Channel { get; set; }
    public string Payload { get; set; }
    
    public PublishPacket(NamespacedId channel, string payload)
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
        Channel = message.Channel.ToId();
        Payload = message.Payload;
    }

    public byte[] Write()
    {
        var message = new Publish
        {
            Channel = Channel.ToName(),
            Payload = Payload
        };

        return message.ToByteArray();
    }
}