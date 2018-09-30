using MvvmHelpers;
using MyDiary.App.Interfaces;
using MyDiary.App.Models;
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

    public partial class MenuPage : ContentPage
    {
        MainMasterDetailPage mainMasterDetailPage;
        List<MasterDetailMenuItem> menuItems;
        private IMyDiaryService azureService;
        public MenuPage(MainMasterDetailPage mainMasterDetailPage)
        {
            azureService = azureService ?? Locator.Current.GetService<IMyDiaryService>();
            this.mainMasterDetailPage = mainMasterDetailPage;
            InitializeComponent();

         
            BindingContext = new BaseViewModel
            {
                Title = "My Diary",
                Subtitle = "",
                Icon = "slideout.png"
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

           
          
            if (CrossSecureStorage.Current.HasKey(App.UserIdKey))
            {
                ListViewMenu.ItemsSource = menuItems = new List<MasterDetailMenuItem>
                {
                    new MasterDetailMenuItem { Title = "About", MenuType = MenuType.About, Icon ="about.png" },
                    new MasterDetailMenuItem { Title = "My Diary", MenuType = MenuType.Diary, Icon = "logo.png" },
                      new MasterDetailMenuItem { Title = "Search Diary Entries", MenuType = MenuType.Search, Icon = "search.png" },
                    new MasterDetailMenuItem { Title = "My Profile", MenuType = MenuType.MyProfile, Icon = "userInfo.png" },
                    new MasterDetailMenuItem { Title = "Logout", MenuType = MenuType.LogOut, Icon = "logout.png" },
                };
            }
            else
            {
                ListViewMenu.ItemsSource = menuItems = new List<MasterDetailMenuItem>
                {
                    new MasterDetailMenuItem { Title = "About", MenuType = MenuType.About, Icon ="about.png" },
                    new MasterDetailMenuItem { Title = "My Diary", MenuType = MenuType.Diary, Icon = "logo.png" },
                      new MasterDetailMenuItem { Title = "Search Diary Entries", MenuType = MenuType.Search, Icon = "search.png" },
                    new MasterDetailMenuItem { Title = "My Profile", MenuType = MenuType.MyProfile, Icon = "userInfo.png" },
                };
            }


            ListViewMenu.SelectedItem = menuItems[0];

            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (ListViewMenu.SelectedItem == null)
                    return;

                if (CrossSecureStorage.Current.HasKey(App.UserIdKey))
                {
                if (((MasterDetailMenuItem)e.SelectedItem).MenuType == MenuType.LogOut)
                    {
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
                else
                    {
                        await this.mainMasterDetailPage.NavigateAsync((int)((MasterDetailMenuItem)e.SelectedItem).MenuType);
                    }
                }
                else
                {
                    await this.mainMasterDetailPage.NavigateAsync((int)(MenuType.Login));
                }
            };
        }
    }
}
