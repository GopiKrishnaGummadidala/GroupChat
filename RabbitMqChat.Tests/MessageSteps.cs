using RabbitMqChat.Contracts;
using System;
using TechTalk.SpecFlow;

namespace RabbitMqChat.Tests
{
    [Binding]
    public class MessageSteps
    {
        private string _groupName;
        private string _memberName;
        private string _text;
        private IMessageGroup _messageGroup;
        private IMessageMember _messageMember;
        private readonly IMessageGroupFactory _messageGroupFactory;

        public MessageSteps()
        {
            _messageGroupFactory = new MessageGroupFactory(null);
        }

        [Given(@"entered some ""(.*)"" message")]
        public void GivenEnteredSomeMessage(string p0)
        {
            
        }
        
        [When(@"Send message in the Group")]
        public void WhenSendMessageInTheGroup()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the result should return a message  with text ""(.*)""")]
        public void ThenTheResultShouldReturnAMessageWithText(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the result should have (.*) in Messages Count")]
        public void ThenTheResultShouldHaveInMessagesCount(int p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
