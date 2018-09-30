using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyDiary.App.Models
{
    [Microsoft.WindowsAzure.MobileServices.DataTable("User")]
    public class User
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string AuthenticationProvidor { get; set; }
    }
}
