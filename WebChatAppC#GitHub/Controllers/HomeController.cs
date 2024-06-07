using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using WebChatAppC_GitHub.Models;
using System.IO;

namespace WebChatAppC_GitHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string usersFolderFilePath = Path.Combine(Directory.GetCurrentDirectory(), "users", "user.json");
        private readonly string messagesFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "messages"); // Corrected

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            EnsureFilesExist();
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var users = new List<RegisterModel>();

            if (System.IO.File.Exists(usersFolderFilePath))
            {
                var existingUsersJson = System.IO.File.ReadAllText(usersFolderFilePath);
                users = JsonSerializer.Deserialize<List<RegisterModel>>(existingUsersJson);
            }

            if (users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return View(model);
            }

            var userToSave = new RegisterModel
            {
                Username = model.Username,
                Password = model.Password
            };

            users.Add(userToSave);
            var updatedUsersJson = JsonSerializer.Serialize(users);
            System.IO.File.WriteAllText(usersFolderFilePath, updatedUsersJson);

            HttpContext.Session.SetString("LoggedInUser", model.Username);
            return RedirectToAction("Chat", new { sessionId = HttpContext.Session.Id });
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var users = new List<RegisterModel>();

            if (System.IO.File.Exists(usersFolderFilePath))
            {
                var existingUsersJson = System.IO.File.ReadAllText(usersFolderFilePath);
                users = JsonSerializer.Deserialize<List<RegisterModel>>(existingUsersJson);
            }

            var validUser = users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

            if (validUser != null)
            {
                var sessionId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("LoggedInUser", model.Username);
                HttpContext.Session.SetString("SessionId", sessionId);
                return RedirectToAction("Chat", new { sessionId });
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        [HttpGet]
        public IActionResult Chat(string sessionId)
        {
            var sessionUser = HttpContext.Session.GetString("LoggedInUser");
            var sessionStoredId = HttpContext.Session.GetString("SessionId");

            if (string.IsNullOrEmpty(sessionUser) || sessionStoredId != sessionId)
            {
                return RedirectToAction("Login");
            }

            var users = new List<RegisterModel>();

            if (System.IO.File.Exists(usersFolderFilePath))
            {
                var existingUsersJson = System.IO.File.ReadAllText(usersFolderFilePath);
                users = JsonSerializer.Deserialize<List<RegisterModel>>(existingUsersJson);
            }

            users = users.Where(u => !string.IsNullOrEmpty(u.Username)).ToList();

            ViewBag.Users = users;
            ViewBag.LoggedInUser = sessionUser;
            ViewBag.SessionId = sessionId;

            var messages = LoadMessages("Group");
            ViewBag.Messages = messages.Where(m => m.Receiver == "Group").ToList();

            return View();
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public JsonResult ChatMessages()
        {
            var sessionUser = HttpContext.Session.GetString("LoggedInUser");
            var groupMessages = LoadMessages("Group");
            var privateMessages = LoadMessages(sessionUser);

            var filteredMessages = groupMessages
                .Concat(privateMessages.Where(m => m.Receiver == sessionUser || m.Sender == sessionUser))
                .ToList();

            return Json(filteredMessages);
        }

        private void EnsureFilesExist()
        {
            var usersDirectoryPath = Path.GetDirectoryName(usersFolderFilePath);
            var messagesDirectoryPath = messagesFolderPath;

            if (!Directory.Exists(usersDirectoryPath))
            {
                Directory.CreateDirectory(usersDirectoryPath);
            }

            if (!System.IO.File.Exists(usersFolderFilePath) || !IsValidJsonArray(System.IO.File.ReadAllText(usersFolderFilePath)))
            {
                System.IO.File.WriteAllText(usersFolderFilePath, "[]");
            }

            if (!Directory.Exists(messagesDirectoryPath))
            {
                Directory.CreateDirectory(messagesDirectoryPath);
            }

            if (!System.IO.File.Exists(Path.Combine(messagesDirectoryPath, "Group.json")) || !IsValidJsonArray(System.IO.File.ReadAllText(Path.Combine(messagesDirectoryPath, "Group.json"))))
            {
                System.IO.File.WriteAllText(Path.Combine(messagesDirectoryPath, "Group.json"), "[]");
            }

            var users = LoadUsers();
            foreach (var user in users)
            {
                var userMessagesPath = Path.Combine(messagesDirectoryPath, $"{user.Username}.json");
                if (!System.IO.File.Exists(userMessagesPath) || !IsValidJsonArray(System.IO.File.ReadAllText(userMessagesPath)))
                {
                    System.IO.File.WriteAllText(userMessagesPath, "[]");
                }
            }
        }

        private bool IsValidJsonArray(string json)
        {
            json = json.Trim();
            return json.StartsWith("[") && json.EndsWith("]");
        }

        private List<Message> LoadMessages(string chatType)
        {
            var chatPath = Path.Combine(messagesFolderPath, $"{chatType}.json");
            if (System.IO.File.Exists(chatPath))
            {
                var messagesJson = System.IO.File.ReadAllText(chatPath);
                return JsonSerializer.Deserialize<List<Message>>(messagesJson);
            }
            return new List<Message>();
        }

        private List<RegisterModel> LoadUsers()
        {
            if (System.IO.File.Exists(usersFolderFilePath))
            {
                var usersJson = System.IO.File.ReadAllText(usersFolderFilePath);
                return JsonSerializer.Deserialize<List<RegisterModel>>(usersJson);
            }
            return new List<RegisterModel>();
        }
    }
}
