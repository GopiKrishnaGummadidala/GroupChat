Feature: MessageGroupFactory
	     Creating a MessageGroup from factory and testing all possible scenarios
		 As a user 
		 I want to be able to create groups and add members and sending messages among groups

@mytag
Scenario: Creating a new group
	Given a group name "CSE" which is not existed in the group list
	When a group with the name "CSE" shall be created
	Then a new group with the name "CSE" is created and returned
	And the group is added and the result should be 1 group in group list


Scenario: Creating a group which is already existed
	Given a group name "CSE" is already existed in the group list
	When a group with the name "CSE" shall be created
	Then an existing group with the name "CSE" is returned
	And the result should be 1 group in group list and list will not be modified


Scenario: Create multiple groups
	Given a group name "ECE" is not existed in the group list
	And also given another group name "IT" is not existed in the group list
	When multiple groups created and returned
	And the result should be 2 groups in group list and list will be modified


Scenario: Joining a member to an existing Group
	Given a member name "Test1" which is not existed in the member list under CSE group
	When the member is joining to CSE group
	Then a new member with the name "Test1" is created under CSE group and returned
	And the member is added and the result should be 1 member in group member list under CSE group


Scenario: Joining multiple members to an existing Group
	Given a member name "Test1" which is not existed in the member list under CSE group
	And also given another member name "Test2" which is not existed in the member list under CSE group
	When multiple members are joining to CSE group are created and returned
	And the members are added and the result should be 2 members in group member list under CSE group


Scenario: Multiple members with same name are trying to join a group
	Given a member name "Test1" which is already existed in the member list under CSE group
	When the member is trying to join to CSE group
	Then the result throws an exception Cannot initialize message member, There is already a member with this name
	And the result should be 1 member in group member list under CSE group and the list will not be modified


Scenario: A member posts a message in Group
	Given a member Test1 entered a message "Hi,,"
	When Test1 member posts that message in CSE group
	Then the result should be return a message with Text "Hi,,"
	And the result should be 1 message in group messages count
	And the result should be 1 member in group members count

Scenario: Member sends multiple messages in Group
	Given a member Test1 also entered a message "Hello,,"
	When Test1 member also posts again in CSE group
	Then result should be return a message with Text "Hello,,"
	And the result should be 2 messages in group messages count
	And the result should be 1 member in group members count


Scenario: A message is deleting from a group
	Given a member Test1 entered a message "Hello,,"
	When Test1 member deletes the message from CSE group
	Then the result should be 1 message in group messages count
	And the result should be 1 member in group members count


Scenario: A member is exiting from a group
	Given a member Test1 entered a message "exit"
	When Test1 member exited from CSE group
	Then the result should be 0 member in group members count
	And the result should be 0 messages in group messages count


Scenario: A group is exiting from group list
	Given a group named CSE 
	When the group exiting from the list of groups
	Then the result should be 0 in group list count


Scenario Outline: Trying to create a group with empty or null 
	Given a group with name <groupname>
	And trying to create a group
	Then the result throws an exception Cannot initialize group, without name

	Examples: 
	| groupname |
	|           |
	| null      |


Scenario Outline: Trying to create a member with empty or null 
	Given a member with name <membername>
	And trying to join an existing group
	Then the result throws an exception Cannot initialize message member, without name

	Examples: 
	| membername |
	|            |
	| null       |