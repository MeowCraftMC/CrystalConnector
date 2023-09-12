using CrystalConnector;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddHostedService<CrystalService>();
});

builder.ConfigureAppConfiguration((context, config) =>
{
    config.Sources.Clear();
    config.AddJsonFile("config.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"config.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddCommandLine(args)
        .AddEnvironmentVariables();
});

builder.UseContentRoot(Environment.CurrentDirectory);

builder.ConfigureWebHost(webHost =>
{
    webHost.UseStartup<Startup>();
    
    webHost.UseKestrel(kestrel =>
    {
    });
});

builder.ConfigureLogging(logging =>
{
    logging.ClearProviders()
        .SetMinimumLevel(LogLevel.Trace)
        .AddNLog();
});

using var host = builder.Build();

await host.RunAsync();
