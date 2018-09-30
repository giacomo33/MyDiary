using MyDiary.App.Interfaces;
using MyDiary.App.Services;
using MyDiary.App.ViewModels;
using MyDiary.App.Views;
using ReactiveUI;
using ReactiveUI.XamForms;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MyDiary.App
{
    public class AppBootstrapper
    {


        public AppBootstrapper()
        {

            RegisterViews();
            RegisterViewModels();
            RegisterServices();


        }


        private void RegisterViews()
        {


        }

        private void RegisterViewModels()
        {

        }

        private void RegisterServices()
        {
            Locator.CurrentMutable.RegisterLazySingleton(() => new AzureService(), typeof(IMyDiaryService));
        }


        public MainMasterDetailPage CreateMainPage()
        {
            var mainMasterDetailPage= new MainMasterDetailPage();
            
            return mainMasterDetailPage;
        }
    }
}
