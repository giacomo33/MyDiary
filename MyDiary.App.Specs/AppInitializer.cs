using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace MyDiary.App.Specs
{
    public static class AppInitializer
    {
        private static string AndroidPath = @"C:\Users\gm\source\repos\MyDiary\MyDiary.App\MyDiary.App.Android\bin\Debug\com.companyname.MyDiary.App.apk";

        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android
                    .ApkFile(AndroidPath)
                    .StartApp();
            }
         
            return ConfigureApp.iOS.StartApp();
        }
    }
}