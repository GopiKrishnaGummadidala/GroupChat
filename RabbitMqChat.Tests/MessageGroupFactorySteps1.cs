using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMqChat.Contracts;
using System;
using TechTalk.SpecFlow;

namespace RabbitMqChat.Tests
{
    public class MessageGroupFactorySteps1
    {
        private string GroupName;
        private IMessageGroup messageGroup;
        private readonly IMessageGroupFactory messageGroupFactory = new MessageGroupFactory(null);

        [Given(@"I have entered ""(.*)"" into console as group name")]
        public void GivenIHaveEnteredIntoConsoleAsGroupName(string name)
        {
            GroupName = name;
        }
        
        [When(@"I click on enter")]
        public void WhenIClickOnEnter()
        {
            messageGroup = messageGroupFactory.Create(GroupName);
        }
        
        [Then(@"the result should be return a message group with name CSE")]
        public void ThenTheResultShouldBeReturnAMessageGroupWithNameCSE()
        {
            Assert.AreEqual(GroupName, messageGroup.Name);
        }
        
        [Then(@"the result should be (.*) on the MessageGroupList Count")]
        public void ThenTheResultShouldBeOnTheMessageGroupListCount(int count)
        {
            Assert.AreEqual(count, messageGroupFactory.GroupList.Count);
        }
    }
}
