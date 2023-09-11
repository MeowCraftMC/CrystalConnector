using CrystalConnector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<CrystalService>();

builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();

builder.Configuration.AddJsonFile("config.json", true)
    .AddJsonFile($"config.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables(prefix: "CRYSTAL_")
    .AddCommandLine(args);

builder.Logging.ClearProviders()
    .SetMinimumLevel(LogLevel.Trace)
    .AddNLog();

using var host = builder.Build();

await host.RunAsync();
