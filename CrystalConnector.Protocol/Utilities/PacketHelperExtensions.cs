using CrystalConnector.Protocol.Messages;

namespace CrystalConnector.Protocol.Utilities;

public static class PacketHelperExtensions
{
    public static (string Namespace, string Name) ToTuple(this NamespacedName name)
    {
        return (name.Namespace, name.Name);
    }
    
    public static NamespacedName ToName(this (string Namespace, string Name) name)
    {
        return new NamespacedName
        {
            Namespace = name.Namespace,
            Name = name.Name
        };
    }
    
    public static string ToIdString(this (string Namespace, string Name) name)
    {
        return $"{name.Namespace}:{name.Name}";
    }
}