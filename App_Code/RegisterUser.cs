using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EGovTesting.App_Code
{
    public class RegisterUser
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public RegisterUser(string username, string password)
        {
            username = Username;
            password = Password;
        }
    }
}
