using System.Net.WebSockets;
using CrystalConnector.Configs;
using CrystalConnector.Connector;
using CrystalConnector.Protocol.Messages;
using CrystalConnector.Protocol.Packets;
using CrystalConnector.Protocol.Packets.C2S;
using CrystalConnector.Protocol.Packets.S2C;
using CrystalConnector.Protocol.Utilities;
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
    
    public async Task<IPacket> Handle(WebSocket webSocket, byte[] data)
    {
        var packet = IPacket.From(data);
        switch (packet)
        {
            case AuthenticatePacket authenticatePacket:
            {
                var clientId = authenticatePacket.ClientId.ToId();
                
                if (authenticatePacket.Key == Config.GetSecretKey())
                {
                    if (WebSocketConnectionManager.HaveClientRegistered(clientId))
                    {
                        Logger.LogInformation("Client {Id} tried authorize with name {Name}, but it is already authorized!", clientId, authenticatePacket.ClientName);
                        return new ResultPacket(Error.NameExists);
                    }
                    
                    Logger.LogInformation("Client {Name}({Id}) authorized", authenticatePacket.ClientName, clientId);

                    var connectionInfo = webSocket.GetConnectionInfo();
                    
                    connectionInfo.Authenticated = true;
                    connectionInfo.ClientId = clientId;
                    connectionInfo.Name = authenticatePacket.ClientName;
                    
                    var channels = connectionInfo.RegisteredChannels;

                    foreach (var channel in authenticatePacket.Channels)
                    {
                        var id = channel.Id.ToId();
                        if (channels.TryGetValue(id, out var chan))
                        {
                            chan.Direction = channel.Direction;
                        }
                        else
                        {
                            channels.Add(id, new MessageChannel
                            {
                                Id = id,
                                Direction = channel.Direction
                            });
                        }
                    }
                    
                    return new ResultPacket();
                }

                Logger.LogWarning("Client {Name}({Id}) auth failed with wrong key!", authenticatePacket.ClientName, clientId);
                return new ResultPacket(Error.Unauthenticated);
            }
            case PublishPacket publishPacket:
            {
                var connectionInfo = webSocket.GetConnectionInfo();
                var id = publishPacket.Channel;

                if (!connectionInfo.Authenticated)
                {
                    return new ResultPacket(Error.Unauthenticated);
                }
                
                if (!connectionInfo.RegisteredChannels.TryGetValue(id, out var registered)
                    || registered.Direction == MessageDirection.Outgoing 
                    || registered.Direction == MessageDirection.All)
                {
                    Logger.LogWarning("Channel {Channel}(Direction: {Direction}) have not been registered by Client {Name}({Id})", 
                        id, MessageDirection.Outgoing, webSocket.GetConnectionInfo().Name, webSocket.GetConnectionInfo().Id);
                    return new ResultPacket(Error.UnregisteredChannel);
                }

                var name = webSocket.GetConnectionInfo().Name;
                await WebSocketConnectionManager.Broadcast(name!, id, publishPacket.Payload);

                if (Config.IsDebug())
                {
                    Logger.LogDebug("Client {Name}({Id}) send data in {Channel}: {Data}", 
                        name, webSocket.GetConnectionInfo().ClientId, id, publishPacket.Payload);
                }

                return new ResultPacket();
            }
            case ForwardPacket forwardPacket:
            {
                break;
            }
            case ResultPacket resultPacket:
            {
                break;
            }
        }
        
        return new ResultPacket();
    }
}