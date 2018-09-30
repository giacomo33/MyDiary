using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using MyDiary.App.Interfaces;
using UIKit;

namespace MyDiary.App.iOS
{
    class LoginProvidoriOS : ILoginProvidor
    {
        public Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(provider, null);
        }
    }
}