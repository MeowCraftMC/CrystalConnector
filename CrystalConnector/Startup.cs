using CrystalConnector.Utilities;
using CrystalConnector.WebSockets;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrystalConnector;

public class Startup
{
    private IConfiguration Config { get; }
    
    private ILogger<Startup> Logger { get; set; }
    private IWebSocketManager WebSocketManager { get; set; }

    public Startup(IConfiguration config)
    {
        Config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        
        Logger = provider.GetService<ILogger<Startup>>()!;
        WebSocketManager = provider.GetService<IWebSocketManager>()!;
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseWebSockets();
        
        app.Use(async (context, next) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var websocket = await context.WebSockets.AcceptWebSocketAsync();
                Logger.LogInformation("Got WebSocket connection from {Ip}:{Port}({Id})", context.Connection.RemoteIpAddress?.ToString(), context.Connection.RemotePort, context.Connection.Id);

                var taskCompletionSource = new TaskCompletionSource<WebSocketHandleResult>();
                await WebSocketManager.StartHandle(context, websocket, taskCompletionSource);
                websocket.Purge();
            }
            else
            {
                await next(context);
            }
        });
    }
}