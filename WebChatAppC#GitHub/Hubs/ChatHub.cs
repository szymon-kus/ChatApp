using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace WebChatAppC_GitHub.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();

        public override Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Session.GetString("LoggedInUser");
            if (!string.IsNullOrEmpty(username))
            {
                _userConnections[Context.ConnectionId] = username;
                Clients.All.SendAsync("ReceiveMessage", "System", $"{username} has joined the chat.");
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _userConnections.TryRemove(Context.ConnectionId, out var username);
            if (!string.IsNullOrEmpty(username))
            {
                Clients.All.SendAsync("ReceiveMessage", "System", $"{username} has left the chat.");
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            // Send message to all clients
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendPrivateMessage(string sender, string receiverUsername, string message)
        {
            var receiverConnectionId = _userConnections.FirstOrDefault(u => u.Value == receiverUsername).Key;

            if (!string.IsNullOrEmpty(receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", sender, message);
                await Clients.Caller.SendAsync("ReceivePrivateMessage", sender, message); // Echo back to sender
            }
        }
    }
}