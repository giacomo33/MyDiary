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

namespace MyDiary.App.Views
{

    public partial class MyProfile : ContentPage
    {
        MyProfileViewModel myProfileViewModel;
        public MyProfile()
        {
            InitializeComponent();
            this.BindingContext = myProfileViewModel = new MyProfileViewModel(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (myProfileViewModel == null)
                return;

            this.myProfileViewModel.LoadUserProfile.Execute(null);
        }
    }
}