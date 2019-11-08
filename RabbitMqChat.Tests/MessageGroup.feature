Feature: MessageGroup
	
@mytag
Scenario: Trying to create group with an empty name 
	Given I have entered "" into console as groupname 
	When  I click enter 
	Then the result should be "Cannot initialize group, without Name"
