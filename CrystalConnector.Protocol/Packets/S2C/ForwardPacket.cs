using CrystalConnector.Protocol.Entities;
using CrystalConnector.Protocol.Messages;
using CrystalConnector.Protocol.Utilities;
using Google.Protobuf;

namespace CrystalConnector.Protocol.Packets.S2C;

public class ForwardPacket : IPacket
{
    public string Publisher { get; set; }
    public NamespacedId Channel { get; set; }
    public string Payload { get; set; }

    public ForwardPacket(string publisher, NamespacedId channel, string payload)
    {
        Publisher = publisher;
        Channel = channel;
        Payload = payload;
    }
    
    public ForwardPacket(byte[] bytes)
    {
        Read(bytes);
    }

    public ForwardPacket()
    {
    }
    
    public void Read(byte[] bytes)
    {
        var message = Forward.Parser.ParseFrom(bytes);
        Publisher = message.Publisher;
        Channel = message.Channel.ToId();
        Payload = message.Payload;
    }

    public byte[] Write()
    {
        var message = new Forward
        {
            Publisher = Publisher,
            Channel = new NamespacedName
            {
                Namespace = Channel.Namespace,
                Name = Channel.Name
            },
            Payload = Payload
        };
        
        return message.ToByteArray();
    }
}