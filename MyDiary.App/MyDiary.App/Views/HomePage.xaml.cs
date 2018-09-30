using MyDiary.App.ViewModels;
using MyDiary.App.Views;
using Plugin.SecureStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyDiary.App
{

    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async void SignInButton_Tapped(object sender, EventArgs e)
        {
            if (CrossSecureStorage.Current.HasKey(App.UserIdKey))
            {
                await Navigation.PushAsync(new NavigationPage(new DiaryListPage())
                {
                    BarBackgroundColor = Color.FromHex("#03A9F4"),
                    BarTextColor = Color.White
                });
            }
            else
            {
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = Color.FromHex("#03A9F4"),
                    BarTextColor = Color.White
                });
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Setup SignIn or View entries Process
            if (CrossSecureStorage.Current.HasKey(App.UserIdKey))
            {
                this.SignInLabel.IsVisible = false;
            }
            else
            {
                this.SignInLabel.Text = "SIGN IN";
            }

            this.SignInButton.Tapped += SignInButton_Tapped;
        }
    }
}