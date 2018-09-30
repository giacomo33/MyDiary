using Microsoft.WindowsAzure.MobileServices;
using MyDiary.App.Interfaces;
using MyDiary.App.UWP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(LoginProvidorWin))]
namespace MyDiary.App.UWP
{
    class LoginProvidorWin : ILoginProvidor
    {
        public Task LoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(provider, "my-diaryapp");
        }
    }
}
