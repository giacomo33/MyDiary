using MyDiary.App.Interfaces;
using MyDiary.App.Services;
using MyDiary.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinUniversity.Infrastructure;

namespace MyDiary.App
{
    public partial class MainPage : ContentPage
    {
        DiaryViewModel diaryViewModel;

        public MainPage(IDependencyService serviceLocator)
        {
            InitializeComponent();
            diaryViewModel= serviceLocator.Get<DiaryViewModel>(); 
            this.BindingContext = diaryViewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
           await this.diaryViewModel.GetAllEntriesAsync();
        }
    }
}
