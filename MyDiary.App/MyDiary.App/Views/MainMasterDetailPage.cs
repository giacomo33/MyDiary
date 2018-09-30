using MvvmHelpers;
using MyDiary.App.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyDiary.App.Views
{
    public class MainMasterDetailPage : MasterDetailPage
    {
        public static bool IsUWPDesktop { get; set; }
        Dictionary<int, NavigationPage> Pages { get; set; }
        public MainMasterDetailPage()
        {
            if (IsUWPDesktop)
                this.MasterBehavior = MasterBehavior.Popover;

            Pages = new Dictionary<int, NavigationPage>();
            Master = new MenuPage(this);
            BindingContext = new BaseViewModel
            {
                Title = "My Diary",
                Icon = "slideout.png"
            };
            //setup home page
            Pages.Add((int)MenuType.Home, new BaseNavigationPage(new HomePage()));

            Detail = Pages[(int)MenuType.Home];

            InvalidateMeasure();
        }



        public async Task NavigateAsync(int id)
        {

            if (Detail != null)
            {
                if (IsUWPDesktop || Device.Idiom != TargetIdiom.Tablet)
                    IsPresented = false;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(300);
            }

            Page newPage;
            if (!Pages.ContainsKey(id))
            {

                switch (id)
                {
                    case (int)MenuType.About:
                        Pages.Add(id, new BaseNavigationPage(new AboutPage()));
                        break;
                    case (int)MenuType.Home:
                        Pages.Add(id, new BaseNavigationPage(new HomePage()));
                        break;
                    case (int)MenuType.Diary:
                        Pages.Add(id, new BaseNavigationPage(new DiaryListPage()));
                        break;
                    case (int)MenuType.Search:
                        Pages.Add(id, new BaseNavigationPage(new SearchPage()));
                        break;
                    case (int)MenuType.MyProfile:
                        Pages.Add(id, new BaseNavigationPage(new MyProfile()));
                        break;
                    case (int)MenuType.LogOut:
                        Pages.Add(id, new BaseNavigationPage(new LogOutPage()));
                        break;
                    case (int)MenuType.Login:
                        Pages.Add(id, new BaseNavigationPage(new LoginPage()));
                        break;
                }
            }

            newPage = Pages[id];
            if (newPage == null)
                return;

            Detail = newPage;
        }
    }
}

