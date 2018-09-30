using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class DiaryViewModel : ViewModelBase
    {
        #region Private Data
        private DiaryEntry selectedEntry;
        private IMyDiaryService azureService;
        #endregion

        public DiaryViewModel(Page page) : base(page)
        {

            Title = "Diary Entries";
            Icon = "blog.png";
            azureService = azureService ?? Locator.Current.GetService<IMyDiaryService>();

            FillUserDetails();


            //Logout = Command.Create(OnClearAuthAsync);

        }

        private Command loadEntries;
        public Command LoadEntries =>
            loadEntries ?? (loadEntries = new Command(async () => await LoadDiaryEntriesAsync()));

        public Command addEntry { get; private set; }
        public Command AddEntry =>
            addEntry ?? (addEntry = new Command(async () => await OnAddEntryAsync()));

        public Command syncEntries { get; private set; }
        public Command SyncEntries =>
            syncEntries ?? (syncEntries = new Command(async () => await OnReSyncEntries()));


        /// <summary>
        /// Command to refresh our display of DiaryEntries from the DB.
        /// </summary>
        public Command Refresh { get; private set; }

        /// <summary>
        /// Command to select a specific DiaryEntry and display the details.
        /// </summary>
        public Command SelectEntry { get; private set; }

        // Lab3: add Logout command
        /// <summary>
        /// Command to Log out of Azure and force a full login event
        /// </summary>


        /// <summary>
        /// List of Diary Entries displayed to the user.
        /// </summary>
        private ObservableCollection<DiaryEntry> entries;
        public ObservableCollection<DiaryEntry> Entries
        {
            get
            {
                return entries;
            }

            set
            {
                SetProperty(ref entries, value);
            }

        }

       

        public DiaryEntry SelectedEntry
        {
            get { return selectedEntry; }
            set
            {
                SetProperty(ref selectedEntry, value);
            }
        }

        User userDetails;
        public User UserDetails
        {
            get
            {
                return userDetails;
            }
            set
            {
                if (userDetails != value)
                {
                    userDetails = value;
                    SetProperty(ref userDetails, value);
                }
            }
        }

        private async Task OnReSyncEntries()
        {
            this.IsBusy = true;
            await azureService.ReSyncAllEntires();
            IsBusy = false;
        }


        private async Task OnAddEntryAsync()
        {
            DiaryEntry entry = new DiaryEntry();
            SelectedEntry = entry;
            await this.page?.Navigation.PushAsync(new DiaryEntryPage(new EntryViewModel( entry,this.page)));
        }

        private async Task OnClearAuthAsync()
        {
            await azureService.LogOffAsync();
            SelectedEntry = null;
        }

        public Color TintColor = Color.FromHex("#03A9F4");
        public Color TintTextColor = Color.White;

        public async Task LoadDiaryEntriesAsync()
        {
            IsBusy = true;
            Entries = new ObservableCollection<DiaryEntry>(await azureService.GetAllEntry());
            IsBusy = false;
        }

        public async void FillUserDetails()
        {
            UserDetails = await azureService.GetUser();
        }
    }
}
