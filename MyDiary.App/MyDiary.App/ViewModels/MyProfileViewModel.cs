using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvvmHelpers;
using MyDiary.App.Interfaces;
using MyDiary.App.Models;
using MyDiary.App.Services;
using Plugin.SecureStorage;
using ReactiveUI;
using Splat;
using Xamarin.Forms;

namespace MyDiary.App.ViewModels
{
    public class MyProfileViewModel : ViewModelBase
    {
        #region Private Data
        private IMyDiaryService azureService;
        #endregion

        public MyProfileViewModel(Page page) : base(page)
        {

            Title = "My Profile";
            Icon = "blog.png";
            azureService = azureService ?? Locator.Current.GetService<IMyDiaryService>();

        }

        private Command loadUserProfile;
        public Command LoadUserProfile =>
            loadUserProfile ?? (loadUserProfile = new Command(async () => await FillUserDetailsAsync()));


        User userDetails;
        public User UserDetails
        {
            get
            {
                return userDetails;
            }
            set
            {
                SetProperty(ref userDetails, value);
            }
        }

        string authenticationProvidor;
        public string AuthenticationProvidor
        {
            get
            {
                return authenticationProvidor;
            }
            set
            {
                SetProperty(ref authenticationProvidor, value);
            }
        }


        async Task FillUserDetailsAsync()
        {
            IsBusy = true;
            UserDetails = await azureService.GetUser();
            IsBusy = false;
        }

    }
}
