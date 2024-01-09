using CrystalConnector.Protocol.Messages;
using Google.Protobuf;

namespace CrystalConnector.Protocol.Packet.S2C;

public class ForwardPacket : IPacket
{
    public string Publisher { get; set; }
    public NamespacedName Channel { get; set; }
    public string Payload { get; set; }

    public ForwardPacket(string publisher, (string Namespace, string Name) channel, string payload)
    {
        Publisher = publisher;
        Channel = new NamespacedName
        {
            Namespace = channel.Namespace,
            Name = channel.Name
        };
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
        Channel = message.Channel;
        Payload = message.Payload;
    }

    public byte[] Write()
    {
        var message = new Forward
        {
            Publisher = Publisher,
            Channel = Channel,
            Payload = Payload
        };
        
        return message.ToByteArray();
    }
}