using System.Net;
using System.Net.WebSockets;

namespace CrystalConnector.Utilities;

public static class WebSocketExtension
{
    private static Dictionary<WebSocket, WebSocketData> Data { get; } = new();

    public static void AddData(this WebSocket webSocket, string id, IPEndPoint ip, 
        TaskCompletionSource<WebSocketHandleResult> taskCompletionSource,
        CancellationTokenSource receiveCancellationTokenSource)
    {
        Data.Add(webSocket, new WebSocketData
        {
            Id = id, 
            IpEndPoint = ip,
            CompletionSource = taskCompletionSource,
            ReceiveCancellationTokenSource = receiveCancellationTokenSource
        });
    }

    public static WebSocketData GetData(this WebSocket webSocket)
    {
        return Data[webSocket];
    }

    public static void RemoveData(this WebSocket webSocket)
    {
        var data = GetData(webSocket);
        data.ReceiveCancellationTokenSource.Cancel();
        data.CompletionSource.SetResult(WebSocketHandleResult.Successful);
        
        Data.Remove(webSocket);
    }

    public static void DisconnectAll()
    {
        foreach (var (socket, _) in Data)
        {
            RemoveData(socket);
        }
    }
}