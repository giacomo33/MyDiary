using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Splat;
using Xamarin.Forms;
using ReactiveUI;
using System.Collections;
using System.Collections.Generic;
using MyDiary.App.Interfaces;
using MyDiary.App.Models;
using MyDiary.App.ViewModels;

namespace MyDiary.App.Views
{

    public partial class SearchPage : ContentPage
    {
        readonly CompositeDisposable eventSubscriptions = new CompositeDisposable();
        

        public SearchPage()
        {
            InitializeComponent();
            this.BindingContext = this;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //convert events into observables
            var textChangedObservable =
                Observable
                .FromEventPattern<TextChangedEventArgs>(
                    x => this.textEntry.TextChanged += x,
                    x => this.textEntry.TextChanged -= x)
                    //filter values emiited from events
                    .Select(x => x.EventArgs.NewTextValue)
                    //throttle the data, this means that only the last value will be sent after a delay
                    .Throttle(TimeSpan.FromSeconds(.75), TaskPoolScheduler.Default);

            var buttonClickObservable =
                Observable
                .FromEventPattern(
                    x => search.Clicked += x,
                    x => search.Clicked -= x)
                    .Select(_ => textEntry.Text);

            this.

             eventSubscriptions.Add(
                 //merge two or more streams into a single one
                 Observable.Merge(textChangedObservable, buttonClickObservable)
                 .Where(searchText => !string.IsNullOrWhiteSpace(searchText))
                 //.BindTo(this.activityIndicator, this.IsBusy, x => x.IsRunning)
                  .Select(searchText =>
                   Observable
                   .FromAsync(
                       async cancellationToken =>
                       {
                           this.IsBusy = true;
                           this.activityIndicatorStack.IsVisible = true;
                           var azureService = Locator.CurrentMutable.GetService<IMyDiaryService>();
                           var azureResults = await azureService.Search(searchText,
                               cancellationToken).ConfigureAwait(false);

                           if (cancellationToken.IsCancellationRequested)
                               throw new TaskCanceledException();

                     
                           return azureResults;
                       })
                       .Catch(Observable.Return(Enumerable.Empty<DiaryEntry>())))
                       .Switch()
                       //create a subscription as a way of gathering our results
                       .Subscribe(
                       results =>
                       {
                           this.IsBusy = false;
                           Device.BeginInvokeOnMainThread(() =>
                           {
                               try
                               {
                                   search.IsEnabled = false;
                                   searchResults.ItemsSource = results.ToList<DiaryEntry>();
                               }
                               finally
                               {
                                   search.IsEnabled = true;
                                   this.IsBusy = false;
                                   this.activityIndicatorStack.IsVisible = false;
                               }
                           });
                          
                       })
                     );

            searchResults.ItemTapped += (sender, args) =>
            {
                if (searchResults.SelectedItem == null)
                    return;
                this.Navigation.PushAsync(new DiaryEntryPage(new EntryViewModel(args.Item as DiaryEntry, this)));
                searchResults.SelectedItem = null;
            };
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            eventSubscriptions.Clear();
            this.textEntry.Text = "";
            //this.searchResults.ItemsSource = null;
        }
    }
}