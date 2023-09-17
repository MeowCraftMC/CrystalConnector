namespace CrystalConnector.Handlers;

public class HandlerConstants
{
    public static readonly string OperationAuthenticate = "Authenticate";
    public static readonly string OperationRegisterChannel = "Register";
    public static readonly string OperationPublish = "Publish";
    
    public static readonly string ResponseSuccessful = "Successful";
    public static readonly string ResponseAuthenticated = "Authenticated";

    public static readonly string ResponseErrorUnknownPacket = "Unknown";
    public static readonly string ResponseErrorUnauthenticated = "Unauthenticated";
    public static readonly string ResponseErrorUndefinedDirection = "UndefinedDirection";
    public static readonly string ResponseErrorUnregisteredDirection = "UnregisteredDirection";
}