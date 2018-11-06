using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

/*
 https://robgibbens.com/bdd-tests-with-xamarin-uitest-and-specflow/
 https://marcofolio.net/automate-specs-specflow-xamarin-uitest/
 https://docs.microsoft.com/en-us/appcenter/test-cloud/uitest/working-with-repl?tabs=vswin
 https://www.youtube.com/results?search_query=%23ExecuteAutomation
 https://insanelab.com/blog/mobile-development/xamarin-test-cloud-guide/
 https://docs.microsoft.com/en-us/appcenter/test-cloud/uitest/
 https://www.youtube.com/watch?v=SsU8vg1_g0s&feature=youtu.be&t=7m28s

  */

namespace MyDiary.App.Specs
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class MyDiaryUITests
    {
        IApp app;
        Platform platform;

        public MyDiaryUITests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
            //app.Repl();
        }

        [Test]
        public void ViewDiaryEntriesList()
        {
            //arrange
            Func<AppQuery, AppQuery> masterDetailsHomeButton = e => e.Marked("OK");
            Func<AppQuery, AppQuery> masterDetailsDiaryEntriesLink = e => e.Marked("My Diary Entries");
            Func<AppQuery, AppQuery> listView = e => e.Id("listView");

            //act
            app.WaitForElement(masterDetailsHomeButton, "Timed out waiting for master details page");
            app.Tap(masterDetailsHomeButton);
            app.WaitForElement(masterDetailsDiaryEntriesLink);
            app.Tap(masterDetailsDiaryEntriesLink);

            //assert
            var listEntries = app.Query(listView).SingleOrDefault();
            Assert.IsNotNull(listEntries);
        }
    }
}
