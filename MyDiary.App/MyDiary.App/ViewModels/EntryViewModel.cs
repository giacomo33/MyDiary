using MvvmHelpers;
using MyDiary.App.Interfaces;
using MyDiary.App.Models;
using Plugin.SecureStorage;
using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyDiary.App.ViewModels
{
    public class EntryViewModel : ViewModelBase
    {
        private IMyDiaryService azureService;
        private readonly DiaryEntry diaryEntry;
        private bool canSave;

        public EntryViewModel(DiaryEntry diaryEntry, Page page) : base(page)
        {
            this.diaryEntry = diaryEntry;
            CanSave = false;

            Title = "Diary Entry";
            Icon = "blog.png";
            azureService = azureService ?? Locator.Current.GetService<IMyDiaryService>();
        }

        public EntryViewModel(Page page)
            : this(new DiaryEntry(), page)
        {
        }

        public Command saveEntry;
        public Command SaveEntry =>
            saveEntry ?? (saveEntry = new Command(async () => await OnSaveEntryAsync(),
                () => { return CanSave && !IsBusy; }));

        public Command deleteEntry;
        public Command DeleteEntry =>
            deleteEntry ?? (deleteEntry = new Command(async () => await OnDeleteEntryAsync(diaryEntry),
                () => { return !IsNew && !IsBusy; }));


        public DiaryEntry Model { get { return diaryEntry; } }
        public bool IsNew { get { return diaryEntry.Id == null; } }


        private async Task OnDeleteEntryAsync(object entry)
        {
            if (this.diaryEntry != null)
            {
                if (await this.page?.DisplayAlert(
                        "Delete Diary Entry", "Are you sure you wish to Delete this entry?", "Yes", "No"))
                {
                    this.IsBusy = true;
                    await azureService.DeleteEntry(this.diaryEntry);
                    this.IsBusy = false;
                    this.page?.Navigation.PopAsync();
                }
            }
        }

        private async Task OnSaveEntryAsync()
        {
            if (this.CanSave)
            {
                this.IsBusy = true;
                if (this.IsNew)
                {
                    diaryEntry.UserId = CrossSecureStorage.Current.GetValue(":UserId");
                    await azureService.PostEntry(this.diaryEntry);
                }
                else
                {
                    await azureService.PatchEntry(this.diaryEntry.Id, this.diaryEntry);
                }
                this.IsBusy = false;
                this.page?.Navigation.PopAsync();
            }
        }

        public string ShortTitle
        {
            get
            {
                return this.diaryEntry.Title == null
                    ? "New Entry"
                    : diaryEntry.Title;
            }
        }

        public string diaryTitle;
        public string DiaryTitle
        {
            get
            { return diaryEntry.Title; }

            set
            {
                if (diaryEntry.Title != value)
                {
                    diaryEntry.Title = value;
                    diaryTitle = value;
                    SetProperty(ref diaryTitle, value);
                    CheckValid();
                }
            }
        }

        public DateTime createdOn;
        public DateTime CreatedOn
        {
            get
            { return diaryEntry.CreatedOn; }

            set
            {
                if (diaryEntry.CreatedOn != value)
                {
                    diaryEntry.CreatedOn = value;
                    createdOn = value;
                    SetProperty(ref createdOn, value);
                    CheckValid();
                }
            }
        }


        public string description;
        public string Description
        {
            get
            {
                return diaryEntry.Description;
            }

            set
            {
                if (diaryEntry.Description != value)
                {
                    diaryEntry.Description = value;
                    description = value;
                    SetProperty(ref description, value);
                    CheckValid();
                }
            }
        }

        void CheckValid()
        {
            CanSave = !string.IsNullOrEmpty(DiaryTitle) && !string.IsNullOrEmpty(Description);
        }

        public bool CanSave
        {
            get
            {
                return canSave;
            }
            set
            {
                SetProperty(ref canSave, value);
            }
        }
    }
}
