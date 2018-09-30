using MvvmHelpers;
using MyDiary.App.Enum;
using MyDiary.App.Interfaces;
using MyDiary.App.Models;
using MyDiary.App.Views;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyDiary.App.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private IMyDiaryService azureService;
        public ReactiveCommand OpenFaceBookAuth { get; private set; }
        public ReactiveCommand OpenGoogleAuth { get; private set; }
        public ReactiveCommand OpenTwitterAuth { get; private set; }
        public ReactiveCommand OpenMSAuth { get; private set; }

        public LoginViewModel(Page page) : base(page)
        {

            OpenFaceBookAuth = ReactiveCommand.Create(OpenFaceBookAuthAsync);
            OpenGoogleAuth = ReactiveCommand.Create(OpenGoogleAuthAsync);
            OpenTwitterAuth = ReactiveCommand.Create(OpenTwitterAuthAsync);
            OpenMSAuth = ReactiveCommand.Create(OpenMSAuthAsync);

            azureService = azureService ?? Locator.Current.GetService<IMyDiaryService>();
        }

        private async Task OpenFaceBookAuthAsync()
        {
            this.IsBusy = true;
            App.AuthenticatorProvidor = AuthenticatorProvidor.Facebook;
            await azureService.InitializeAsync();
            await azureService.LoginAsync();
            App.Current.MainPage = new MainMasterDetailPage();
            this.IsBusy = false;
        }

        private async Task OpenGoogleAuthAsync()
        {
            this.IsBusy = true;
            App.AuthenticatorProvidor = AuthenticatorProvidor.Google;
            await azureService.InitializeAsync();
            await azureService.LoginAsync();
            this.page?.Navigation.PopModalAsync();
            App.Current.MainPage = new MainMasterDetailPage();
            this.IsBusy = false;
        }

        private async Task OpenTwitterAuthAsync()
        {
            this.IsBusy = true;
            App.AuthenticatorProvidor = AuthenticatorProvidor.Twitter;
            await azureService.InitializeAsync();
            await azureService.LoginAsync();
            App.Current.MainPage = new MainMasterDetailPage();
            this.IsBusy = false;
        }

        private async Task OpenMSAuthAsync()
        {
            this.IsBusy = true;
            App.AuthenticatorProvidor = AuthenticatorProvidor.Microsoft;
            await azureService.InitializeAsync();
            await azureService.LoginAsync();
            App.Current.MainPage = new MainMasterDetailPage();
            this.IsBusy = false;
        }
    }
}
