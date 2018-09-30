using Microsoft.Azure.Mobile.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MyDiary.DataObjects
{
    public class User : EntityData
    {
        [Column("UserId")]
        public string UserId { get; set; }
        [Column("FirstName")]
        public string FirstName { get; set; }
        [Column("LastName")]
        public string LastName { get; set; }
        [Column("Email")]
        public string Email { get; set; }
        [Column("AuthenticationProvidor")]
        public string AuthenticationProvidor { get; set; }
    }
}