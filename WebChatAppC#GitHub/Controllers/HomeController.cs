using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;
using WebChatAppC_GitHub.Models;

namespace WebChatAppC_GitHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        //Sciezka do folderu i pliku users
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

            // If ModelState is valid, save user data to JSON file
            var users = new List<RegisterModel>();

            if (System.IO.File.Exists(usersFolderFilePath))
            {
                var existingUsersJson = System.IO.File.ReadAllText(usersFolderFilePath);
                users = JsonSerializer.Deserialize<List<RegisterModel>>(existingUsersJson);
            }

            //zapis username i password
            var userToSave = new RegisterModel
            {
                Username = model.Username,
                Password = model.Password
            };

            //dodajemy i przeksztalcenie formatu json na tekst
            users.Add(model);
            var updatedUsersJson = JsonSerializer.Serialize(users);
            System.IO.File.WriteAllText(usersFolderFilePath, updatedUsersJson);

            // Po zapisaniu uzytkownika redirect na strone glowna z chatem

            return RedirectToAction("Chat", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Chat()
        {
            return View();
        }

        // Tworzenie foldera Users i pliku Json jesli nie istnieja w projekcie
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

    //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    //    public IActionResult Error()
    //    {
    //        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    //    }
    //}
}
