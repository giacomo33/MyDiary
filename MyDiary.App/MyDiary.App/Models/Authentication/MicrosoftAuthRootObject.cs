using System;
using System.Collections.Generic;
using System.Text;

namespace MyDiary.App.Models.Authentication
{
    public class MicrosoftAuthRootObject
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string AccessTokenExpiration { get; set; }
        public List<MicrosoftAuth> UserClaims { get; set; }
    }
}
