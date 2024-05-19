using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using WebChatAppC_GitHub.Models;

namespace WebChatAppC_GitHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly string usersFolderFilePath = Path.Combine(Directory.GetCurrentDirectory(), "users", "user.json");

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            usersFolderFilePathExists();
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

            var userToSave = new RegisterModel
            {
                Username = model.Username,
                Password = model.Password
            };

            users.Add(model);
            var updatedUsersJson = JsonSerializer.Serialize(users);
            System.IO.File.WriteAllText(usersFolderFilePath, updatedUsersJson);

            HttpContext.Session.SetString("LoggedInUser", model.Username);
            return RedirectToAction("Chat", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(User model)
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
                HttpContext.Session.SetString("LoggedInUser", model.Username);
                return RedirectToAction("Chat", "Home");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View(model);
        }

        [HttpGet]
        public IActionResult Chat()
        {
            var users = new List<RegisterModel>();

            if (System.IO.File.Exists(usersFolderFilePath))
            {
                var existingUsersJson = System.IO.File.ReadAllText(usersFolderFilePath);
                users = JsonSerializer.Deserialize<List<RegisterModel>>(existingUsersJson);
            }

            var loggedInUser = HttpContext.Session.GetString("LoggedInUser");
            ViewBag.Users = users;
            ViewBag.LoggedInUser = loggedInUser;
            return View();
        }

        private void usersFolderFilePathExists()
        {
            var directoryPath = Path.GetDirectoryName(usersFolderFilePath);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!System.IO.File.Exists(usersFolderFilePath))
            {
                System.IO.File.WriteAllText(usersFolderFilePath, "[]"); // pusty array json
            }
        }
    }
}
