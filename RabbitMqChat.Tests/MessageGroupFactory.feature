Feature: MessageGroupFactory
	Creating a Messagegroupfactory and testing all possible scenarios

Scenario Outline: Create group
	Given I have entered <GroupName> into console as group name
	When I click on enter
	Then the result should be return a message group with name <ResultGroupName>
	And the result should be <GroupListCount> on the MessageGroupList Count

Examples:
    | GroupName | ResultGroupName | GroupListCount |
    |  "CSE"    |      "CSE"      |       1        |
	|  "CSE"    |      "CSE"      |       1        |
	|  "IT"     |      "IT"       |       2        |
	|  ""       |      "exception"|       0        |


Scenario Outline: Join a Member to Group
	Given I have entered <Name> into console as member name
	When I click on enter
	Then the result should be return a message member with name <ResultName>
	And the result should be <GroupMembersCount> on the MessageGroupList Count

Examples:
    |   Name   | ResultName     | GroupMembersCount |
    |  "Test"  |   "Test"       |       1           |
	|  "Test"  |   "exception"  |       1           |
	|  ""      |   "exception"  |       0           |


Scenario Outline: Send a message in Group
	Given I have entered <Name> into console as member name
	When I click on enter
	Then the result should be return a message member with name <ResultName>
	And the result should be <GroupMembersCount> on the MessageGroupList Count

Examples:
    |   Name   | ResultName     | GroupMembersCount |
    |  "Test"  |   "Test"       |       1           |
	|  "Test"  |   "exception"  |       1           |
	|  ""      |   "exception"  |       0           |