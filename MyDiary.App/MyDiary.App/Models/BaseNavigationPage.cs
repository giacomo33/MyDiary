using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MyDiary.App.Models
{
    public class BaseNavigationPage : NavigationPage
    {
        public BaseNavigationPage(Page root) : base(root)
        {
            Init();
        }

        public BaseNavigationPage()
        {
            Init();
        }

        void Init()
        {

            BarBackgroundColor = Color.FromHex("#03A9F4");
            BarTextColor = Color.White;
        }
    }
}
