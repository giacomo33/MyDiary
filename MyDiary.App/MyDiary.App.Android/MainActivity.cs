using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using MyDiary.App.Interfaces;
using Plugin.SecureStorage;
using System.Collections.Generic;

namespace MyDiary.App.Droid
{
    [Activity(Label = "MyDiary.App", MainLauncher = true, Icon ="@drawable/icon",
         ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAuthenticate
    {
        // Define a authenticated user.
        private MobileServiceUser user;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            ProtectedFileImplementation.StoragePassword = Build.Id;
            SQLitePCL.Batteries.Init();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            // Initialize the authenticator before loading the app.
            App.Init((IAuthenticate)this);

            LoadApplication(new App());
        }

        public async Task<bool> Authenticate(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                user = await client.LoginAsync(this,
                    provider, "my-diaryapp");
                if (user != null)
                {
                    message = string.Format("you are now signed-in as {0}.",
                        user.UserId);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage(message);
            builder.SetTitle("Sign-in result");
            builder.Create().Show();

            return success;
        }

        public Task PlatformLoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(this, provider, "my-diaryapp", new Dictionary<string, string> {
                        { "access_type", "offline" }
            });
        }
    }
}