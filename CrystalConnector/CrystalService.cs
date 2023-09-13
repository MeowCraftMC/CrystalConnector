using CrystalConnector.Connector;
using CrystalConnector.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;

namespace CrystalConnector;

public class CrystalService : IHostedService
{
    private ILogger<CrystalService> Logger { get; }
    private IConfiguration Config { get; }
    
    public CrystalService(ILogger<CrystalService> logger, IConfiguration config, IHostApplicationLifetime appLifetime)
    {
        Logger = logger;
        Config = config;

        appLifetime.ApplicationStarted.Register(OnStarted);
        appLifetime.ApplicationStopping.Register(OnStopping);
        appLifetime.ApplicationStopped.Register(OnStopped);
    }
    
    private void OnStarted()
    {
        Logger.LogInformation("Starting!");
    }

    private void OnStopping()
    {
        Logger.LogInformation("Stopping!");
        WebSocketConnectionManager.DisconnectAll();
    }
    
    private void OnStopped()
    {
        Logger.LogInformation("Goodbye!");
        LogManager.Shutdown();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}