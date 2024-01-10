using CrystalConnector.Protocol.Messages;

namespace CrystalConnector.Protocol.Entities;

public record NamespacedId(string Namespace, string Name)
{
    public NamespacedName ToName()
    {
        return new NamespacedName
        {
            Namespace = Namespace,
            Name = Name
        };
    }

    public override string ToString()
    {
        return $"{Namespace}:{Name}";
    }
}