using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OI.MVC.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string RoleType { get; set; }
    }
}