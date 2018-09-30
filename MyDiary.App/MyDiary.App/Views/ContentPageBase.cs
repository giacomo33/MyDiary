using ReactiveUI;
using ReactiveUI.XamForms;
using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using Xamarin.Forms;

namespace MyDiary.App.Views
{
    public class ContentPageBase<TViewModel> : ReactiveContentPage<TViewModel> where TViewModel : class
    {
        protected readonly CompositeDisposable SubscriptionDisposables = new CompositeDisposable();

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            SubscriptionDisposables.Clear();
        }
    }
}
