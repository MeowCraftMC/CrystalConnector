using System.Formats.Cbor;
using System.Net.WebSockets;
using CrystalConnector.Configs;
using CrystalConnector.Connector;
using CrystalConnector.Connector.Packet;
using CrystalConnector.Connector.Packet.S2C;
using CrystalConnector.Utilities;
using Microsoft.Extensions.Logging;

namespace CrystalConnector.Handlers;

public class WebSocketHandler
{
    private ConfigAccessor Config { get; }

    private ILogger<WebSocketHandler> Logger { get; }
    
    public WebSocketHandler(ConfigAccessor config, ILogger<WebSocketHandler> logger)
    {
        Config = config;
        Logger = logger;
    }
    
    public async Task<PacketS2C> Handle(WebSocket webSocket, CborReader reader)
    {
        reader.ReadStartArray();
        
        // Todo: qyl27: Use C2SPacketReader.
        var type = reader.ReadTextString();
        if (type == HandlerConstants.OperationAuthenticate)
        {
            if (!webSocket.GetConnectionInfo().Authenticated)
            {
                var secret = reader.ReadTextString();
                var name = reader.ReadTextString();
                if (secret == Config.GetSecretKey())
                {
                    if (WebSocketConnectionManager.HaveNameRegistered(name))
                    {
                        Logger.LogInformation("Client {Id} tried authorize with name {Name}, but it is already authorized!", webSocket.GetConnectionInfo().Id, name);
                        return new S2CNameAuthenticatedPacket();
                    }
                    
                    Logger.LogInformation("Client {Name}({Id}) authorized", name, webSocket.GetConnectionInfo().Id);
                    webSocket.GetConnectionInfo().Authenticated = true;
                    webSocket.GetConnectionInfo().Name = name;
                    return new S2CSuccessfulPacket();
                }

                Logger.LogWarning("Client {Name}({Id}) auth failed with wrong key!", name, webSocket.GetConnectionInfo().Id);
                return new S2CUnauthenticatedPacket();
            }

            return new S2CAuthenticatedPacket();
        }

        if (type == HandlerConstants.OperationRegisterChannel)
        {
            if (!webSocket.GetConnectionInfo().Authenticated)
            {
                Logger.LogWarning("Client {Id} is unauthenticated!", webSocket.GetConnectionInfo().Id);
                return new S2CUnauthenticatedPacket();
            }

            var channelId = reader.ReadTextString();
            var directionValue = reader.ReadInt32();

            if (!Enum.IsDefined(typeof(MessageDirection), directionValue))
            {
                Logger.LogWarning("Client {Name}({Id}) tried to register channel {Channel} with undefined direction {Direction}!", 
                    webSocket.GetConnectionInfo().Name, webSocket.GetConnectionInfo().Id, channelId, directionValue);
                return new S2CUndefinedDirectionPacket();
            }
            
            var direction = (MessageDirection)directionValue;

            var channels = webSocket.GetConnectionInfo().RegisteredChannels;
            if (channels.TryGetValue(channelId, out var channel))
            {
                channel.Direction |= direction;
            }
            else
            {
                webSocket.GetConnectionInfo().RegisteredChannels.Add(channelId, new MessageChannel
                {
                    Id = channelId,
                    Direction = direction
                });
            }
            
            Logger.LogInformation("Client {Name}({Id}) registered channel {Channel}(Direction: {Direction})", 
                webSocket.GetConnectionInfo().Name, webSocket.GetConnectionInfo().Id, channelId, direction.ToString());
            return new S2CSuccessfulPacket();
        }

        if (type == HandlerConstants.OperationPublish)
        {
            var channelId = reader.ReadTextString();
            var payload = reader.ReadByteString();

            if (!webSocket.GetConnectionInfo().RegisteredChannels.TryGetValue(channelId, out var value)
                || !value.Direction.HasFlag(MessageDirection.Outgoing))
            {
                Logger.LogWarning("Channel {Channel}(Direction: {Direction}) have not been registered by Client {Name}({Id})", 
                    channelId, MessageDirection.Outgoing, webSocket.GetConnectionInfo().Name, webSocket.GetConnectionInfo().Id);
                return new S2CUnregisteredDirectionPacket();
            }

            try
            {
                var name = webSocket.GetConnectionInfo().Name;
                await WebSocketConnectionManager.Broadcast(name!, channelId, payload);

                if (Config.IsDebug())
                {
                    Logger.LogDebug("Client {Name}({Id}) send data in {Channel}: {Data}", 
                        name, webSocket.GetConnectionInfo().Id, channelId, Convert.ToBase64String(payload));
                }
                return new S2CSuccessfulPacket();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Send error!");
            }
        }

        reader.ReadEndArray();
        
        return new S2CUnknownPacket();
    }
}