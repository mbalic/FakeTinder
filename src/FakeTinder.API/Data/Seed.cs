using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using FakeTinder.API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace FakeTinder.API.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        public Seed(UserManager<User> userManager)
        {
            this._userManager = userManager;
        }

        public void SeedUsers()
        {
            if (!this._userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);

                foreach (var user in users)
                {
                    this._userManager.CreateAsync(user, "password").Wait();
                }
            }
        }
    }
}