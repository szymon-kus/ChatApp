using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks;
using WebChatAppC_GitHub.Models;

namespace WebChatAppC_GitHub.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, bool> _onlineUsers = new ConcurrentDictionary<string, bool>();
        private readonly string messagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "messages");

        public override async Task OnConnectedAsync()
        {
            var username = Context.GetHttpContext().Session.GetString("LoggedInUser");
            if (!string.IsNullOrEmpty(username))
            {
                _userConnections[Context.ConnectionId] = username;
                _onlineUsers[username] = true;
                await Clients.All.SendAsync("UpdateOnlineStatus", _onlineUsers);
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_userConnections.TryRemove(Context.ConnectionId, out var username))
            {
                _onlineUsers.TryRemove(username, out _);
                await Clients.All.SendAsync("UpdateOnlineStatus", _onlineUsers);
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            var chatMessage = new Message
            {
                Sender = user,
                Receiver = "Group",
                Content = message,
                Timestamp = DateTime.Now
            };
            SaveMessage(chatMessage, "Group");

            await Clients.All.SendAsync("ReceiveMessage", user, message);

            var notificationMessage = $"{user} sent a message to the group chat.";
            foreach (var connection in _userConnections)
            {
                if (connection.Value != user)
                {
                    await Clients.Client(connection.Key).SendAsync("ReceiveNotification", notificationMessage);
                }
            }
        }

        public async Task SendPrivateMessage(string sender, string receiverUsername, string message)
        {
            var receiverConnectionId = _userConnections.FirstOrDefault(u => u.Value == receiverUsername).Key;

            var chatMessage = new Message
            {
                Sender = sender,
                Receiver = receiverUsername,
                Content = message,
                Timestamp = DateTime.Now
            };
            SaveMessage(chatMessage, receiverUsername);
            SaveMessage(chatMessage, sender);

            if (!string.IsNullOrEmpty(receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", sender, message);
            }
            await Clients.Caller.SendAsync("ReceivePrivateMessage", sender, message);

            var notificationMessage = $"{sender} sent you a message.";
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveNotification", notificationMessage);
        }

        private void SaveMessage(Message message, string chatType)
        {
            var chatPath = Path.Combine(messagesFolderPath, $"{chatType}.json");
            var messages = new List<Message>();

            if (File.Exists(chatPath))
            {
                var messagesJson = File.ReadAllText(chatPath);
                messages = JsonSerializer.Deserialize<List<Message>>(messagesJson);
            }

            messages.Add(message);
            var updatedMessagesJson = JsonSerializer.Serialize(messages);
            File.WriteAllText(chatPath, updatedMessagesJson);
        }

        public async Task SendNotification(string receiverUsername, string notificationMessage)
        {
            var receiverConnectionId = _userConnections.FirstOrDefault(u => u.Value == receiverUsername).Key;
            if (!string.IsNullOrEmpty(receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveNotification", notificationMessage);
            }
        }
    }
}
