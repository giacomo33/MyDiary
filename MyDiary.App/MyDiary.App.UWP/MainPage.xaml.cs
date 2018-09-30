using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Windows.UI.Popups;
using MyDiary.App.Interfaces;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace MyDiary.App.UWP
{
    public sealed partial class MainPage : IAuthenticate
    {
        public MainPage()
        {
            this.InitializeComponent();

            MyDiary.App.App.Init(this);
            MyDiary.App.App.IsWindows10 = true;
            MyDiary.App.Views.MainMasterDetailPage.IsUWPDesktop = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Desktop";
            LoadApplication(new MyDiary.App.App());

            ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(800, 600));
        }

        // Define a authenticated user.
        private MobileServiceUser user;

        public async Task<bool> Authenticate(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            string message = string.Empty;
            var success = false;

            try
            {
                // Sign in with Providor login using a server-managed flow.
                if (client.CurrentUser == null)
                {
                    ((App)Application.Current).Client = client;
                    user = await client
                        .LoginAsync(provider, "my-diaryapp", new Dictionary<string, string> {
                        { "access_type", "offline" }
            });
                    if (user != null)
                    {
                        success = true;
                        message = string.Format("You are now signed-in as {0}.", user.UserId);
                    }
                }

            }
            catch (Exception ex)
            {
                message = string.Format("Authentication Failed: {0}", ex.Message);
            }

            // Display the success or failure message.
            await new MessageDialog(message, "Sign-in result").ShowAsync();

            return success;
        }

        public Task PlatformLoginAsync(MobileServiceClient client, MobileServiceAuthenticationProvider provider)
        {
            return client.LoginAsync(provider, "my-diaryapp", new Dictionary<string, string> {
                        { "access_type", "offline" }
            });
        }
    }
}
