using MvvmHelpers;
using MyDiary.App.Enum;
using MyDiary.App.Interfaces;
using Plugin.SecureStorage;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyDiary.App.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {

        public ReactiveCommand SignInButtonCommand { get; private set; }


        public HomeViewModel()
        {
            //SignInButtonCommand = ReactiveCommand.Create(SignInButtonCommandAsync);
         
        }

        //private void SignInButtonCommandAsync()
        //{
        //    if (CrossSecureStorage.Current.HasKey(App.UserIdKey))
        //    {
        //        HostScreen.Router.NavigateAndReset
        //            .Execute(new testvm())
        //            .Subscribe();
        //    }
        //    else
        //    {
        //        HostScreen.Router.Navigate
        //           .Execute(new LoginViewModel())
        //           .Subscribe();
        //    }
        //}
    }
}
