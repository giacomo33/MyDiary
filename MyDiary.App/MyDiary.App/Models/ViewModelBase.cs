using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MyDiary.App.Models
{
    public class ViewModelBase : BaseViewModel
    {
        protected Page page;
        public ViewModelBase(Page page)
        {
            this.page = page;
        }
    }
}
