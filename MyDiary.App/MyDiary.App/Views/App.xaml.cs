using MyDiary.App.Enum;
using MyDiary.App.Interfaces;
using MyDiary.App.Services;
using MyDiary.App.ViewModels;
using MyDiary.App.Views;
using Plugin.SecureStorage;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MyDiary.App
{
    public partial class App : Application
    {
        public static IAuthenticate Authenticator { get; private set; }
        public static AuthenticatorProvidor AuthenticatorProvidor { get; set; }
        public const string UserIdKey = ":UserId";
        public const string TokenKey = ":Token";
        public static bool IsWindows10 { get; set; }


        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }

        public App()
        {

            InitializeComponent();

            // Register dependencies.
            var bootstrapper = new AppBootstrapper();
            MainPage = bootstrapper.CreateMainPage();

            //INavigationService navService = DependencyService.Get<INavigationService>();

            //// Get the implementation so we can register our pages. Consumers just navigating to a page
            //// won't do this - we only do this as part of our initialization. Probably should use "as" cast here
            //// in real code and make sure that's what you got back too!
            //var rnavService = (FormsNavigationPageService)navService;
            //// Register a real page with the key
            //rnavService.RegisterPage(AppPage.Master, () => new DiaryListPage());
            //// Do some other action on the Navigate - in this case, show the "Master" page.
            //// Return null to cancel the NavigationService action.
            //rnavService.RegisterPage(AppPage.Detail, () => new DiaryEntryPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
