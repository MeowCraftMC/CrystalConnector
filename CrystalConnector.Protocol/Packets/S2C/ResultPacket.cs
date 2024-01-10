using CrystalConnector.Protocol.Messages;
using Google.Protobuf;

namespace CrystalConnector.Protocol.Packets.S2C;

public class ResultPacket : IPacket
{
    public Result.ResultOneofCase ResultOneofCase { get; set; }
    public Error? Error { get; set; }
    
    public ResultPacket()
    {
        ResultOneofCase = Result.ResultOneofCase.Successful;
    }
    
    public ResultPacket(Error error)
    {
        ResultOneofCase = Result.ResultOneofCase.Error;
        Error = error;
    }
    
    public ResultPacket(byte[] bytes)
    {
        Read(bytes);
    }
    
    public void Read(byte[] bytes)
    {
        var message = Result.Parser.ParseFrom(bytes);
        ResultOneofCase = message.ResultCase;

        if (message.HasError)
        {
            Error = message.Error;
        }
    }

    public byte[] Write()
    {
        var message = ResultOneofCase switch
        {
            Result.ResultOneofCase.None => new Result
            {
                Error = Messages.Error.Unknown
            },
            Result.ResultOneofCase.Successful => new Result
            {
                Successful = new Successful()
            },
            Result.ResultOneofCase.Error => new Result
            {
                Error = Error!.Value
            },
            _ => throw new ArgumentOutOfRangeException()
        };

        return message.ToByteArray();
    }
}