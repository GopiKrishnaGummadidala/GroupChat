using EasyNetQ;
using EasyNetQ.Topology;
using RabbitMqChat.Contracts;
using RabbitMqChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using IMessage = RabbitMqChat.Contracts.IMessage;

namespace RabbitMqChat
{
    public class MessageGroupFactory : IMessageGroupFactory
    {
        private IBus Bus { get; }

        public MessageGroupFactory(IBus bus)
        {
            Bus = bus;
        }

        public IMessageGroup Create(string gName, string uName)
        {
            
            if (!CheckForGroup(gName))
            {
                PublishGroupName(gName);
                Console.WriteLine("Group is created with Name : {0}", gName);
            }
            Console.WriteLine("You will be joined to chat soon. If you will want to leave just enter message 'exit'");
            SubscribeMemberJoin(gName, uName);
            SubscribeMemberExit(gName, uName);
            SubscribeMessage(gName, uName);

            return new MessageGroup(Bus,gName);
        }

        private bool CheckForGroup(string gName)
        {
            try
            {
                var response = Bus.Request<GroupRequest, GroupResponse>(new GroupRequest { Name = "Test" });
                if (gName.Equals(response.Name))
                {
                    Console.WriteLine("Group is available with Name : {0}", response.Name);
                }
                return gName.Equals(response.Name);
            }
            catch (Exception e)
            {
                return false;
            }
        }

       

        private void PublishGroupName(string gName)
        {
            Bus.Respond<GroupRequest, GroupResponse>(request => ResponseGroupName(gName)); 
        }

        private GroupResponse ResponseGroupName(string gName)
        {
            return new GroupResponse { Name = gName };
        }

        private void SubscribeMemberJoin(string gName,string uName)
        {
            Bus.Subscribe<MemberJoin>(uName, response =>
            {
                Console.WriteLine("Member : {0} joined to this Group : {1}", response.Name, response.GroupName);
            }, x => x.WithTopic(gName));
        }

        private void SubscribeMemberExit(string gName, string uName)
        {
            Bus.Subscribe<MemberLeave>(uName, response =>
            {
                Console.WriteLine("Member : {0} left from this Group : {1}", response.Name, response.GroupName);
            }, x => x.WithTopic(gName));
        }

        private void SubscribeMessage(string gName, string uName)
        {
            Bus.Subscribe<MemberMessage>(uName, response =>
            {
                Console.WriteLine("{0} > {1}", response.Sender, response.Text);
            }, x => x.WithTopic(gName));

            Bus.Subscribe<MemberMessage>(uName, response =>
            {
                Console.WriteLine("{0} > {1}", response.Sender, response.Text);
            }, x => x.WithTopic(uName));
        }

        public void Dispose()
        {
            Bus?.Dispose();
        }

        private class MessageGroup : IMessageGroup
        {
            public IBus Bus { get; private set; }
            public string Name { get; private set; }

            public IList<IMessage> Messages { get; private set; }

            public IList<IMessageMember> Members { get; private set; }

            public event Action<IMessageMember> MembersChanged;
            public event Action<IMessage> MessageChanged;

            public MessageGroup(IBus bus, string name)
            {
                Bus = bus;
                Name = name;
                Members = new List<IMessageMember>();
                Messages = new List<IMessage>();
            }

            public IMessageMember Join(string uName)
            {
                if (Members.Any(m => m.Name == uName))
                {
                    throw new OperationCanceledException("Cannot initialize message member, There is already a member with this name");
                }

                MessageMember messageMember = new MessageMember(uName, this);
                Members.Add(messageMember);

                SendMemberJoin(Name, uName);

                if (MembersChanged != null)
                {
                    MembersChanged.Invoke(messageMember);
                }
                return messageMember;
            }

            private void SendMemberJoin(string gName, string uName)
            {
                Bus.Publish(new MemberJoin { Name = uName, GroupName = gName }, x => x.WithTopic(gName));
            }

            public bool Exit(string uName)
            {
                if (Members.Any(m => m.Name == uName))
                {
                    var member = Members.FirstOrDefault(m => m.Name == uName);
                    Members.Remove(member);
                    SendMemberExit(Name, uName);
                    if (MembersChanged != null)
                    {
                        MembersChanged.Invoke(member);
                    }
                    return true;
                }
                
                return false;
            }

            private void SendMemberExit(string gName, string uName)
            {
                Bus.Publish(new MemberLeave { Name = uName, GroupName = gName }, x => x.WithTopic(gName));
                DeleteQueue("RabbitMqChat.Models.MemberJoin:RabbitMqChat.Models_" + uName, false);
                DeleteQueue("RabbitMqChat.Models.MemberLeave:RabbitMqChat.Models_" + uName, false);
                DeleteQueue("RabbitMqChat.Models.MemberMessage:RabbitMqChat.Models_" + uName, false);
            }

            private void DeleteQueue(string qName, bool isExclusive)
            {
                try
                {
                    IQueue queue = new Queue(qName, isExclusive);
                    Bus.Advanced.QueueDelete(queue);
                }
                catch (Exception e)
                {
                    //To Do: Exception Logging
                }
            }

            public bool DeleteMessage(string text)
            {
                if (Messages.Any(m => m.Text == text))
                {
                    var message = Messages.FirstOrDefault(m => m.Text == text);
                    Messages.Remove(message);
                    if (MessageChanged != null)
                    {
                        MessageChanged.Invoke(message);
                    }
                    return true;
                }

                return false;
            }

            public void Dispose()
            {
                Members = null;
                Messages = null;
            }
        }

        private class MessageMember : IMessageMember
        {
            public string Name { get; set; }

            public IMessageGroup Group { get; set; }

            public MessageMember(string name, IMessageGroup group)
            {
                Name = name;
                Group = group;
            }

            public void Dispose()
            {
                if (Group.Members.Any(m => m.Name == Name))
                {
                    //foreach (var message in Group.Messages.Where(m => m.Sender == this))
                    //{
                    //    Group.Messages.Remove(message);
                    //}

                    var member = Group.Members.FirstOrDefault(m => m.Name == Name);
                    Group.Members.Remove(member);
                }
            }

            public IMessage Send(string text, string target = null)
            {
                Message msg= new Message(text, this, null, Group);
                Group.Messages.Add(msg);

                MemberMessage publishMessage = new MemberMessage();
                publishMessage.PostedOn = msg.Time;
                publishMessage.GroupName = msg.Group?.Name;
                publishMessage.Sender = msg.Sender?.Name;
                publishMessage.Target = msg.Target?.Name;
                publishMessage.Text = msg.Text;

                if(target != null)
                {
                    Group.Bus.Publish(publishMessage, x => x.WithTopic(target));
                }
                else
                {
                    Group.Bus.Publish(publishMessage, x => x.WithTopic(Group.Name));
                }
               
                return msg;
            }
        }

        private class Message : IMessage
        {
            public IMessageGroup Group { get; set; }

            public IMessageMember Sender { get; set; }

            public IMessageMember Target { get; set; }

            public string Text { get; set; }

            public DateTime Time { get; set; }

            public Message(string text, IMessageMember sender, IMessageMember target, IMessageGroup group)
            {
                Text = text;
                Sender = sender;
                Target = target;
                Group = group;
                Time = DateTime.Now;
            }

            public void Dispose()
            {
                if (Group.Messages.Any(m => m.Text == Text))
                {
                    var message = Group.Messages.FirstOrDefault(m => m.Text == Text);
                    Group.Messages.Remove(message);
                }
            }
        }
    }
}
