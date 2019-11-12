using NUnit.Framework;
using RabbitMqChat.Contracts;
using System;
using TechTalk.SpecFlow;

namespace RabbitMqChat.Tests
{
    [Binding]
    public class MessageGroupFactorySteps
    {
        private string _groupName;
        private string _memberName;
        private IMessageGroup _messageGroup;
        private IMessageMember _messageMember;
        private readonly IMessageGroupFactory _messageGroupFactory;

        public MessageGroupFactorySteps()
        {
            _messageGroupFactory = new MessageGroupFactory(null);
        }

        [Given(@"group ""(.*)""")]
        public void GivenGroup(string name)
        {
            _groupName = name;
        }
        
        [Given(@"member ""(.*)""")]
        public void GivenMember(string name)
        {
            _memberName = name;
        }
        
        [When(@"group is created")]
        public void WhenGroupIsCreated()
        {
            _messageGroup = _messageGroupFactory.Create(_groupName);
        }
        
        [When(@"member joined to group")]
        public void WhenMemberJoinedToGroup()
        {
            if (string.IsNullOrEmpty(_memberName))
            {
                Assert.Throws<ArgumentNullException>(() => _messageGroup.Join(_memberName));
            }
            else
            {
                _messageMember = _messageGroup.Join(_memberName);
            }
        }
        
        [Then(@"result should return a message group with name ""(.*)""")]
        public void ThenResultShouldReturnAMessageGroupWithName(string name)
        {
            Assert.AreEqual(name, _messageGroup.Name);
        }
        
        [Then(@"result should have (.*) in the MessageGroupList Count")]
        public void ThenResultShouldHaveInTheMessageGroupListCount(int count)
        {
            Assert.AreEqual(count, _messageGroupFactory.GroupList.Count);
        }
        
        [Then(@"result should return a message member with name ""(.*)""")]
        public void ThenResultShouldReturnAMessageMemberWithName(string name)
        {
            Assert.AreEqual(name, _messageMember.Name);
        }
        
        [Then(@"result should have (.*) member in MessageGroup members")]
        public void ThenResultShouldHaveMemberInMessageGroupMembers(int count)
        {
            Assert.AreEqual(count, _messageGroup.Members.Count);
        }
    }
}
