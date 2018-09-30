using System;
using System.Collections.Generic;
using System.Text;

namespace MyDiary.App.Models
{

    public enum MenuType
    {
        About,
        Home,
        Diary,
        MyProfile,
        Search,
        LogOut,
        Login
    }
    public class MasterDetailMenuItem : BaseModel
    {
        public MasterDetailMenuItem()
        {
            MenuType = MenuType.Home;
        }
        public string Icon { get; set; }
        public MenuType MenuType { get; set; }
    }
}
