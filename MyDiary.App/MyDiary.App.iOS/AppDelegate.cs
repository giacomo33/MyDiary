﻿using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using MyDiary.App.Interfaces;

namespace MyDiary.App.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate, IAuthenticate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        MobileServiceClient Client;
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(43, 132, 211); //bar background
            UINavigationBar.Appearance.TintColor = UIColor.White; //Tint color of button items
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                Font = UIFont.FromName("HelveticaNeue-Light", (nfloat)20f),
                TextColor = UIColor.White
            });

            global::Xamarin.Forms.Forms.Init();
            App.Init(this);
            LoadApplication(new App());

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            SQLitePCL.Batteries.Init();
            return base.FinishedLaunching(app, options);
        }

        // Define a authenticated user.
        private MobileServiceUser user;

        public async Task<bool> Authenticate(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            var success = false;
            var message = string.Empty;
            try
            {
                // Sign in with Facebook login using a server-managed flow.
                Client = client;
                if (user == null)
                {
                    user = await client
                        .LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController,
                        provider, "my-diaryapp");
                    if (user != null)
                    {
                        message = string.Format("You are now signed-in as {0}.", user.UserId);
                        success = true;
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            // Display the success or failure message.
            UIAlertView avAlert = new UIAlertView("Sign-in result", message, null, "OK", null);
            avAlert.Show();
            
            return success;
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return Client.ResumeWithURL(url);
        }

        public Task PlatformLoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(UIApplication.SharedApplication.KeyWindow.RootViewController,
                        provider, "my-diaryapp", new Dictionary<string, string> {
                        { "access_type", "offline" }
            });
        }
    }
}
