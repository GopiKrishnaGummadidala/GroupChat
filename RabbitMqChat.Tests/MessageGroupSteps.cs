using Microsoft.VisualStudio.TestTools.UnitTesting;
using RabbitMqChat.Contracts;
using System;
using TechTalk.SpecFlow;

namespace RabbitMqChat.Tests
{
    [Binding]
    public class MessageGroupSteps
    {
        private string GroupName;
        private IMessageGroup messageGroup;
        private readonly IMessageGroupFactory messageGroupFactory = new MessageGroupFactory(null);
        [Given(@"I have entered ""(.*)"" into console as groupname")]
        public void GivenIHaveEnteredIntoConsoleAsGroupname(string p0)
        {
            GroupName = p0;
        }
        
        [When(@"I click enter")]
        public void WhenIClickEnter()
        {
            messageGroup = messageGroupFactory.Create(GroupName);
        }
        
        [Then(@"the result should be ""(.*)""")]
        public void ThenTheResultShouldBe(string p0)
        {
            Assert.AreEqual(p0, messageGroup.Name);
        }
    }
}
