using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrystalConnector;

public class Startup
{
    public IConfiguration Config { get; }

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