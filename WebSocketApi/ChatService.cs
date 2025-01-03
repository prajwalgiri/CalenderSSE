using System.Net.WebSockets;
using System.Text;

namespace WebSocketApi
{
    public class ChatService
    {
        private readonly List<WebSocket> _sockets = new();
        private readonly List<(string, WebSocket)> _users = new();
        private readonly List<string> _history = new();
        private readonly IUserService _userService;
        public ChatService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task HandleWebSocketConnection(WebSocket socket, string name, string? toUser = null)
        {
            _sockets.Add(socket);
            var buffer = new byte[1024 * 2];
            if (!_userService.AddUser(name))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "User already exists", default);
                return;
            }
            _users.Add((name, socket));
            _history.Add("New user joined:" + name);
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, default);
                    break;
                }
                if (toUser == null)
                {

                    foreach (var s in _sockets)
                    {
                        _history.Add(Encoding.UTF8.GetString(buffer[..result.Count]));
                        await s.SendAsync(buffer[..result.Count], WebSocketMessageType.Text, true, default);
                    }
                }
                else
                {
                    var user = _users.FirstOrDefault(x => x.Item1 == toUser);
                    if (user.Item2 != null)
                    {
                        await user.Item2.SendAsync(buffer[..result.Count], WebSocketMessageType.Text, true, default);
                    }
                }
                _sockets.Remove(socket);
            }
        }
    }
}
