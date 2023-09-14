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
        return Config.GetValue<bool>("Connector:Debug");
    }

    public string GetSecretKey()
    {
        return Config.GetValue<string>("Connector:Auth:Secret") ?? "";
    }
}