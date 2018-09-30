using MyDiary.App.Interfaces;
using MyDiary.App.Models;
using MyDiary.App.ViewModels;
using MyDiary.App.Views;
using Plugin.SecureStorage;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyDiary.App.Views
{

    public partial class LogOutPage : ContentPage
    {
        private IMyDiaryService azureService;
        public LogOutPage()
        {
            InitializeComponent();
            azureService = azureService ?? Locator.Current.GetService<IMyDiaryService>();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (await this.DisplayAlert("Logout", "Are you sure you wish to logout of this application?", "Yes", "No"))
            {
                this.IsBusy = true;
                await azureService.LogOffAsync();
                this.IsBusy = false;
              
                await Navigation.PushModalAsync(new NavigationPage(new LoginPage())
                {
                    BarBackgroundColor = Color.FromHex("#03A9F4"),
                    BarTextColor = Color.White
                });
            }
        }


    }
}