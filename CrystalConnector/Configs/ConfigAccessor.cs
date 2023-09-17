using Microsoft.Extensions.Configuration;

namespace CrystalConnector.Configs;

public class ConfigAccessor
{
    private IConfiguration Config { get; }
    
    public ConfigAccessor(IConfiguration config)
    {
        Config = config;
    }

    public bool IsDebug()
    {
        return HasDevEnvironmentVariable() || Config.GetValue<bool>("Connector:Debug");
    }

    private bool HasDevEnvironmentVariable()
    {
        var dev = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        return dev != null && dev.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }

    public string GetSecretKey()
    {
        return Config.GetValue<string>("Connector:Auth:Secret") ?? "";
    }
}