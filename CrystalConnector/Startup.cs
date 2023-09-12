using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace CrystalConnector;

public class Startup
{
    private IConfiguration Config { get; }

    private Logger Logger { get; } = LogManager.GetCurrentClassLogger();

    public Startup(IConfiguration config)
    {
        Config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseWebSockets();
        
        app.Use(async (context, next) =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                await context.WebSockets.AcceptWebSocketAsync();
                Logger.Info("Got WebSocket connection from {Ip}:{Port}", context.Connection.RemoteIpAddress?.ToString(), context.Connection.RemotePort);
            }

            await next(context);
        });
    }
}