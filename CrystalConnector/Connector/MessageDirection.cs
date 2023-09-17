namespace CrystalConnector.Connector;

[Flags]
public enum MessageDirection
{
    None = 0,
    Incoming = 1,
    Outgoing = 2,
    All = Incoming | Outgoing
}