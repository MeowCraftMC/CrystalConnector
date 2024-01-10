using CrystalConnector.Protocol.Packets.C2S;
using CrystalConnector.Protocol.Packets.S2C;
using Google.Protobuf;

namespace CrystalConnector.Protocol.Packets;

public interface IPacket
{
    public void Read(byte[] bytes);

    public byte[] Write();

    public static IPacket? From(byte[] data)
    {
        var result = TryForm<AuthenticatePacket>(data);
        if (result is not null)
        {
            return result;
        }
        
        result = TryForm<PublishPacket>(data);
        if (result is not null)
        {
            return result;
        }
        
        result = TryForm<ForwardPacket>(data);
        if (result is not null)
        {
            return result;
        }
        
        result = TryForm<ResultPacket>(data);
        return result;
    }

    private static IPacket? TryForm<T>(byte[] data) where T : IPacket, new()
    {
        try
        {
            var message = new T();
            message.Read(data);
            return message;
        }
        catch (InvalidProtocolBufferException ex)
        {
            return null;
        }
    }
}