using CrystalConnector.Protocol.Entities;
using CrystalConnector.Protocol.Messages;

namespace CrystalConnector.Protocol.Utilities;

public static class NamespacedNameExtensions
{
    public static NamespacedId ToId(this NamespacedName name)
    {
        return new NamespacedId(name.Namespace, name.Name);
    }
}