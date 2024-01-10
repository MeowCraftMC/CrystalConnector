using System.Net.WebSockets;
using System.Reactive.Linq;
using CrystalConnector.Client.Extensions;
using CrystalConnector.Protocol.Entities;
using CrystalConnector.Protocol.Messages;
using CrystalConnector.Protocol.Packets;
using CrystalConnector.Protocol.Packets.C2S;
using CrystalConnector.Protocol.Packets.S2C;
using Microsoft.Extensions.Logging;
using Websocket.Client;

namespace CrystalConnector.Client;

public class ConnectorClient
{
    public delegate void PayloadHandler(ConnectorClient client, ForwardPacket packet);

    public event PayloadHandler? OnPayload;
    
    private WebsocketClient Client { get; }

    private List<Channel> Channels { get; } = new();

    private ILogger? Logger { get; }

    public ConnectorClient(NamespacedId id, string name, string key, 
        IEnumerable<Tuple<NamespacedId, MessageDirection>> channels, Uri uri)
    {
        Client = new WebsocketClient(uri);

        Channels.AddRange(channels.Select(ch => new Channel
        {
            Id = ch.Item1.ToName(),
            Direction = ch.Item2
        }).ToList());
        
        Client.ReconnectTimeout = null;
        Client.ReconnectionHappened.Subscribe(info =>
        {
            Logger?.LogInformation("Connected! Type: {Type}", info.Type);

            var packet = new AuthenticatePacket(id.ToName(), name, key, Channels);
            Client.Send(packet);
        });
        
        Client.MessageReceived.Where(msg => msg.MessageType == WebSocketMessageType.Binary)
            .Subscribe(message =>
            {
                try
                {
                    var packet = IPacket.From(message.Binary!);
                    switch (packet)
                    {
                        case ForwardPacket forwardPacket:
                        {
                            OnPayload?.Invoke(this, forwardPacket);
                            break;
                        }
                        case ResultPacket resultPacket:
                        {
                            if (resultPacket.ResultOneofCase != Result.ResultOneofCase.Successful)
                            {
                                Logger?.LogWarning("Warn {Error}", resultPacket.Error.ToString());
                            }
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger?.LogWarning(ex, "Bad packet!");
                }
            });
    }
    
    public ConnectorClient(NamespacedId id, string name, string key, 
        IEnumerable<Tuple<NamespacedId, MessageDirection>> channels, Uri uri, ILogger logger)
        : this(id, name, key, channels, uri)
    {
        Logger = logger;
    }

    public async Task Start()
    {
        Logger?.LogInformation("Connector connecting...");
        await Client.Start();
    }

    public async Task Stop()
    {
        Logger?.LogInformation("Connector disconnecting...");
        await Client.Stop(WebSocketCloseStatus.NormalClosure, "");
    }

    public void Send(NamespacedId channel, string payload)
    {
        Client.Send(new PublishPacket(channel, payload));
    }
}