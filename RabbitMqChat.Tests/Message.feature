Feature: Message
		 Creating a Message from Message Member and testing all possible scenarios

Scenario: Sending a message
Given group "CSE"
And created a group
Given member "Test"
And member joined to group
And entered some "Hi," message
When Send message in the Group
Then the result should return a message  with text "Hi,"
And the result should have 1 in Messages Count
