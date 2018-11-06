Feature: AddDiaryEntry
add a new diary entry


Scenario: Add new diary entry
Given I opened the app
Then I have opened the My Diary page
Then I press the Add Entry button
Then I enter "testTitle" as the title
Then I enter "testDescription" as the description
When I press the Save Button
Then the My Diary page loads