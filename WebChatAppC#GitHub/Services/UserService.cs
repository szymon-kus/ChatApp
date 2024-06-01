//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Newtonsoft.Json;

//namespace WebChatAppC.Services
//{
//    public class UserService
//    {
//        private readonly string _filePath = "users/user.json";

//        public List<User> GetAllUsers()
//        {
//            var json = File.ReadAllText(_filePath);
//            return JsonConvert.DeserializeObject<List<User>>(json);
//        }

//        public User ValidateUser(string username, string password)
//        {
//            var users = GetAllUsers();
//            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
//        }
//    }
//}