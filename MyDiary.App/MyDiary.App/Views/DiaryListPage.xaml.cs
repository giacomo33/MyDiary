using MyDiary.App.Models;
using MyDiary.App.ViewModels;
using MyDiary.App.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyDiary.App.Views
{

    public partial class DiaryListPage : ContentPage
    {
        DiaryViewModel diaryViewModel;
        public DiaryListPage()
        {
            InitializeComponent();

            BindingContext = diaryViewModel = new DiaryViewModel(this);

            listView.ItemTapped += (sender, args) =>
            {
                if (listView.SelectedItem == null)
                    return;
                this.Navigation.PushAsync(new DiaryEntryPage(new EntryViewModel(args.Item as DiaryEntry,this)));
                listView.SelectedItem = null;
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (diaryViewModel == null)
                return;

            diaryViewModel.LoadEntries.Execute(null);
            //var listViewItemTappedObservable =
            //  Observable
            //  .FromEventPattern<ItemTappedEventArgs>(
            //      i => listView.ItemTapped += i,
            //      i => listView.ItemTapped -= i)
            //     .Subscribe(args =>
            //    Device.BeginInvokeOnMainThread(() =>
            //    {
            //        this.ViewModel.HostScreen.Router.Navigate.Execute(new AboutViewModel()).Subscribe();
            //        this.listView.SelectedItem = null;
            //    }))

            //          .DisposeWith(this.SubscriptionDisposables);

        }




    }
}