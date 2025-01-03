using System.Net.WebSockets;
using System.Text;

namespace WebSocketClient
{
    class Program
    {
        private static readonly string Connection = "wss://localhost:7156/";

        static async Task Main(string[] args)
        {
            do
            {
                using (var socket = new ClientWebSocket())
                    try
                    {
                        Console.WriteLine("Connecting to server...");
                        await socket.ConnectAsync(new Uri(Connection), CancellationToken.None);
                        Console.WriteLine("Connected to server");
                        Console.WriteLine("Enter message...");
                        var msg =Console.ReadLine();
                        
                        Console.WriteLine("Sending data...");
                        await Send(socket, msg);
                        await Receive(socket);

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Connection failed. Retrying in 5 seconds...");
                        Console.WriteLine($"ERROR - {ex.Message}");
                        await Task.Delay(5000);
                        Console.ReadLine();
                    }
            } while (true);
        }

        static async Task Send(ClientWebSocket socket, string data) =>
            await socket.SendAsync(Encoding.UTF8.GetBytes(data), WebSocketMessageType.Text, true, CancellationToken.None);

        static async Task Receive(ClientWebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);
            do
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    ms.Seek(0, SeekOrigin.Begin);
                    using (var reader = new StreamReader(ms, Encoding.UTF8))
                        Console.WriteLine(await reader.ReadToEndAsync());
                }
            } while (true);
        }
    }
}