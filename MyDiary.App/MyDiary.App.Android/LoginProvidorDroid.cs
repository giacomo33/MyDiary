using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using MyDiary.App.Droid;
using MyDiary.App.Interfaces;
using Xamarin.Forms;

[assembly: Xamarin.Forms.Dependency(typeof(LoginProvidorDroid))]
namespace MyDiary.App.Droid
{
    class LoginProvidorDroid : ILoginProvidor
    {
        public Task LoginAsync(MobileServiceClient client,
            MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(MainApplication.CurrentContext, provider, "my-diaryapp");
        }
    }
}