using System.Net;
using System.Security.Cryptography.X509Certificates;
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

builder.ConfigureLogging(logging =>
{
    logging.ClearProviders()
        .SetMinimumLevel(LogLevel.Trace)
        .AddNLog();
});

builder.ConfigureWebHost(webHost =>
{
    webHost.UseStartup<Startup>();

    webHost.UseKestrel();

    webHost.ConfigureKestrel((context, kestrel) =>
    {
        var config = context.Configuration;
        foreach (var listener in config.GetSection("Server:Listener").GetChildren())
        {
            if (listener.GetValue<bool>("Enabled"))
            {
                var ipStr = listener.GetValue<string>("IP");
                if (!IPAddress.TryParse(ipStr, out var ip))
                {
                    continue;
                }
            
                var port = listener.GetValue<int>("Port");

                var https = listener.GetSection("Https");
                if (!https.GetValue<bool>("Enabled") || https.GetValue<string>("Cert") == null)
                {
                    kestrel.Listen(ip, port);
                }
                else
                {
                    var cert = https.GetValue<string>("Cert");
                    var password = https.GetValue<string>("Password");

                    kestrel.Listen(ip, port, listenOptions =>
                    {
                        listenOptions.UseHttps(httpsOptions =>
                        {
                            httpsOptions.ServerCertificate = X509Certificate2.CreateFromEncryptedPemFile(cert!, password);
                        });
                    });
                }
            }
        }
    });
});

using var host = builder.Build();

await host.RunAsync();
