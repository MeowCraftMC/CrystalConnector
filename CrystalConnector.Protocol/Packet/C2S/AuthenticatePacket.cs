using CrystalConnector.Protocol.Messages;
using Google.Protobuf;
using Google.Protobuf.Collections;

namespace CrystalConnector.Protocol.Packet.C2S;

public class AuthenticatePacket : IPacket
{
    public NamespacedName ClientId { get; set; }
    public string ClientName { get; set; }
    public string Key { get; set; }
    
    public List<Channel> Channels { get; } = [];
    
    public AuthenticatePacket(NamespacedName clientId, string clientName, string key, List<Channel> channels)
    {
        ClientId = clientId;
        ClientName = clientName;
        Key = key;
        Channels.AddRange(channels);
    }
    
    public AuthenticatePacket(byte[] bytes)
    {
        Read(bytes);
    }
    
    public AuthenticatePacket()
    {
    }
    
    public void Read(byte[] bytes)
    {
        var message = Authenticate.Parser.ParseFrom(bytes);
        
        ClientId = message.ClientId;
        ClientName = message.ClientName;
        Key = message.SecretKey;
        Channels.AddRange(message.Channels);
    }

    public byte[] Write()
    {
        var message = new Authenticate
        {
            ClientId = ClientId,
            ClientName = ClientName,
            SecretKey = Key
        };
        message.Channels.AddRange(Channels);
        
        return message.ToByteArray();
    }
}