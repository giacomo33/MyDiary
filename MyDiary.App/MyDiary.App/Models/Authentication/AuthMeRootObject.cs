using System;
using System.Collections.Generic;
using System.Text;

namespace MyDiary.App.Models.Authentication
{
    public class AuthMeRootObject
    {
        public string id_token { get; set; }
        public string provider_name { get; set; }
        public List<AuthMe> user_claims { get; set; }
        public string user_id { get; set; }
    }
}
