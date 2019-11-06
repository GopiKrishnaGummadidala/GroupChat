using System;

namespace RabbitMqChat.Models
{
    public class MemberMessage
    {
        public DateTime PostedOn { get; set; }
        public string Sender { get; set; }
        public string Target { get; set; }
        public string GroupName { get; set; }
        public string Text { get; set; }
    }
}
