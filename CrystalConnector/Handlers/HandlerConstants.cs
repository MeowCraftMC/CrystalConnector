namespace CrystalConnector.Handlers;

public class HandlerConstants
{
    public static readonly string OperationAuthenticate = "Authenticate";
    public static readonly string OperationRegisterChannel = "Register";
    public static readonly string OperationPublish = "Publish";
    // Todo: qyl27: Does we need a exit message?
    // public static readonly string OperationExit = "Bye";
    
    public static readonly string ResponseSuccessful = "Successful";
    
    public static readonly string ResponseErrorUnknownPacket = "Unknown";
    public static readonly string ResponseErrorMalformed = "Malformed";
    public static readonly string ResponseErrorAuthenticated = "Authenticated";
    public static readonly string ResponseErrorUnauthenticated = "Unauthenticated";
    public static readonly string ResponseErrorUndefinedDirection = "UndefinedDirection";
    public static readonly string ResponseErrorUnregisteredDirection = "UnregisteredDirection";
}