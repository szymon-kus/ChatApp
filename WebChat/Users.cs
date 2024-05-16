using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace OnlineChat
{
    public class Users
    {
        private readonly string _filePath;
        public Dictionary<string, string> users = new Dictionary<string, string>();
        public string _userName;
        public string _password;
        public List<string> loggedUsers = new List<string>();

        public Users(string filePath)
        {
            _filePath = filePath;
            this._userName = "";
            this._password = "";
            LoadUsersFromJson();
        }

        public void CreateUser()
        {
            Console.WriteLine("Podaj nazwę użytkownika: ");
            this._userName = Console.ReadLine();
            if (users.ContainsKey(this._userName))
            {
                Console.WriteLine("Użytkownik już istnieje.");
                return;
            }
            Console.WriteLine("Podaj hasło: ");
            this._password = Console.ReadLine();
            Console.WriteLine("Potwierdź hasło: ");
            string confirmPassword = Console.ReadLine();
            if (this._password != confirmPassword)
            {
                Console.WriteLine("Potwierdzenie hasła niezgodne.");
                return;
            }
            users.Add(this._userName, this._password);
            SaveUsersToJson();
        }

        public void LoginUser()
        {
            Console.WriteLine("Podaj nazwę użytkownika: ");
            string userName = Console.ReadLine();
    
            if (users.ContainsKey(userName))
            {
                Console.WriteLine("Podaj hasło: ");
                string enteredPassword = Console.ReadLine();
                string storedPassword = users[userName];
                if (enteredPassword == storedPassword)
                {
                    Console.WriteLine("Zalogowano pomyślnie");
                    loggedUsers.Add(userName);
                }
                else
                {
                    Console.WriteLine("Błędne hasło");
                }
            }
            else
            {
                Console.WriteLine("Użytkownik nie istnieje");
            }
        }
        private void LoadUsersFromJson()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                users = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
        }

        private void SaveUsersToJson()
        {
            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(_filePath, json);
        }
    }
}