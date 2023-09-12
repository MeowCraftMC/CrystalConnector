using CrystalConnector.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CrystalConnector;

public class Startup
{
    private IConfiguration Config { get; }
    
    private ILogger<Startup> Logger { get; set; }
    private IWebSocketHandler WebSocketHandler { get; set; }

    public Startup(IConfiguration config)
    {
        Config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        
        Logger = provider.GetService<ILogger<Startup>>()!;
        WebSocketHandler = provider.GetService<IWebSocketHandler>()!;
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseWebSockets();
        
        app.Use(async (context, next) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var websocket = await context.WebSockets.AcceptWebSocketAsync();
                Logger.LogInformation("Got WebSocket connection from {Ip}:{Port}", context.Connection.RemoteIpAddress?.ToString(), context.Connection.RemotePort);

                var taskCompletionSource = new TaskCompletionSource<WebSocketHandleResult>();
                WebSocketHandler.StartHandle(context, websocket, taskCompletionSource);
                await taskCompletionSource.Task;
            }
            else
            {
                await next(context);
            }
        });
    }
}