using MyDiary.App.ViewModels;
using MyDiary.App.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyDiary.App
{

    public partial class DiaryEntryPage : ContentPage
    {
        EntryViewModel entry;
        public DiaryEntryPage(EntryViewModel entry)
        {
            InitializeComponent();
            this.entry = entry;
            this.Title = "Edit Diary Entry";

            this.BindingContext = entry;
        }
    }
}