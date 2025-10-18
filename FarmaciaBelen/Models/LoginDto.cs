using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FarmaciaBelen.Models
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; } // segundos
    }
}