using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using WebChatAppC_GitHub.Models;

namespace WebChatAppC_GitHub.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
        private readonly string messagesFolderFilePath = Path.Combine(Directory.GetCurrentDirectory(), "messages", "messages.json");

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
            var chatMessage = new Message
            {
                Sender = user,
                Receiver = "Group",
                Content = message,
                Timestamp = DateTime.Now
            };
            SaveMessage(chatMessage);

            await Clients.Others.SendAsync("ReceiveMessage", user, message); 
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
            SaveMessage(chatMessage);

            if (!string.IsNullOrEmpty(receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", sender, message);
            }
        }

        private void SaveMessage(Message message)
        {
            var messages = LoadMessages();
            messages.Add(message);
            var messagesJson = JsonSerializer.Serialize(messages);
            File.WriteAllText(messagesFolderFilePath, messagesJson);
        }

        private List<Message> LoadMessages()
        {
            if (File.Exists(messagesFolderFilePath))
            {
                var messagesJson = File.ReadAllText(messagesFolderFilePath);
                return JsonSerializer.Deserialize<List<Message>>(messagesJson);
            }
            return new List<Message>();
        }
    }
}
