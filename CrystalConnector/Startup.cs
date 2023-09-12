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
    }
}