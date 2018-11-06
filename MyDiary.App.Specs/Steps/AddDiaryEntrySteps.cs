using System;
using TechTalk.SpecFlow;
using Xamarin.UITest;

namespace MyDiary.App.Specs
{
    [Binding]
    public class AddDiaryEntrySteps
    {
        readonly IApp app;

        public AddDiaryEntrySteps()
        {
            app = AppInitializer.StartApp(Platform.Android);
        }

        [Given(@"I opened the app")]
        public void GivenIOpenedTheApp()
        {
            app.Repl();
        }

        [When(@"I press the Save Button")]
        public void WhenIPressTheSaveButton()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I have opened the My Diary page")]
        public void ThenIHaveOpenedTheMyDiaryPage()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I press the Add Entry button")]
        public void ThenIPressTheAddEntryButton()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I enter ""(.*)"" as the title")]
        public void ThenIEnterAsTheTitle(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"I enter ""(.*)"" as the description")]
        public void ThenIEnterAsTheDescription(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the My Diary page loads")]
        public void ThenTheMyDiaryPageLoads()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
