using System.Formats.Cbor;
using System.Net.WebSockets;

var webSocket = new ClientWebSocket();
var cborWriter = new CborWriter();

while (true)
{
    try
    {
        await Process();
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }
}

async Task Receive()
{
    var buffer = new byte[1024 * 4];
    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

    while (!result.CloseStatus.HasValue)
    {
        var reader = new CborReader(new ReadOnlyMemory<byte>(buffer, 0, result.Count));
        Console.WriteLine("Response length: " + reader.BytesRemaining);
        reader.ReadStartArray();
        Console.WriteLine("Result: " + reader.ReadTextString());
        
        result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    }
}

async Task Process()
{
    Console.Write("> ");
    var operation = Console.ReadLine();

    if (operation == "")
    {
        return;
    }

    if (operation == "connect")
    {
        var uri = Console.ReadLine();
        await webSocket.ConnectAsync(new Uri(uri!), CancellationToken.None);
        new Thread(() => Receive()).Start();
    }
    else if (operation == "message")
    {
        cborWriter.Reset();
        cborWriter.WriteStartArray(null);
    }
    else if (operation == "send")
    {
        cborWriter.WriteEndArray();
        var bytes = cborWriter.Encode();
        await webSocket.SendAsync(bytes, WebSocketMessageType.Binary, true, CancellationToken.None);
    }
    else if (operation == "bye")
    {
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
        webSocket = new ClientWebSocket();
    }
    else if (operation == "help")
    {
        Console.WriteLine("Every space should use enter.");
        Console.WriteLine("Connect to a server: connect <uri>");
        Console.WriteLine("New message: message");
        Console.WriteLine("Send message: send");
        Console.WriteLine("Close websocket: bye");
        Console.WriteLine("Insert data: data <type> <value>");
    }
    else if (operation == "data")
    {
        var type = Console.ReadLine();
        var valueStr = Console.ReadLine();
        
        if (type == "string")
        {
            cborWriter.WriteTextString(valueStr!);
        }
        else if (type == "byte")
        {
            var bytes = Convert.FromBase64String(valueStr);
            cborWriter.WriteByteString(bytes);
        }
        else if (type == "int")
        {
            var result = int.TryParse(valueStr, out var value);
            while (!result)
            {
                valueStr = Console.ReadLine();
                
                result = int.TryParse(valueStr, out value);
            }
            cborWriter.WriteInt32(value);
        }
        else if (type == "long")
        {
            var result = long.TryParse(valueStr, out var value);
            while (!result)
            {
                valueStr = Console.ReadLine();
                
                result = long.TryParse(valueStr, out value);
            }
            cborWriter.WriteInt64(value);
        }
        else if (type == "bool")
        {
            var result = bool.TryParse(valueStr, out var value);
            while (!result)
            {
                valueStr = Console.ReadLine();
                
                result = bool.TryParse(valueStr, out value);
            }
            cborWriter.WriteBoolean(value);
        }
        else if (type == "float")
        {
            var result = float.TryParse(valueStr, out var value);
            while (!result)
            {
                valueStr = Console.ReadLine();
                
                result = float.TryParse(valueStr, out value);
            }
            cborWriter.WriteSingle(value);
        }
        else if (type == "double")
        {
            var result = double.TryParse(valueStr, out var value);
            while (!result)
            {
                valueStr = Console.ReadLine();
                
                result = double.TryParse(valueStr, out value);
            }
            cborWriter.WriteDouble(value);
        }
        else if (type == "decimal")
        {
            var result = decimal.TryParse(valueStr, out var value);
            while (!result)
            {
                valueStr = Console.ReadLine();
                
                result = decimal.TryParse(valueStr, out value);
            }
            cborWriter.WriteDecimal(value);
        }
        else if (type == "null")
        {
            cborWriter.WriteNull();
        }
    }

    Console.WriteLine("Done!");
}
