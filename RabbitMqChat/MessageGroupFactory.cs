using EasyNetQ;
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
            SendMemberJoined(gName, uName);
            SubscribeMemberJoined(gName, uName);
            SubscribeMessage(gName, uName);
            return new MessageGroup(Bus,gName);
        }

        private bool CheckForGroup(string gName)
        {
            try
            {
                var response = Bus.Request<GroupRequest, GroupResponse>(new GroupRequest { Name = "Test" });
                Console.WriteLine("Group is available with Name : {0}", response.Name);
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

        private void SubscribeMemberJoined(string gName,string uName)
        {
            Bus.Subscribe<MemberJoined>(uName, response =>
            {
                Console.WriteLine("Member : {0} joined to this Group : {1}", response.Name, response.GroupName);
            }, x => x.WithTopic(gName));
        }

        private void SendMemberJoined(string gName, string uName)
        {
            Bus.Publish(new MemberJoined { Name = uName, GroupName = gName }, x => x.WithTopic(gName));
        }

        private void SubscribeMessage(string gName, string uName)
        {
            Bus.Subscribe<MemberMessage>(uName, response =>
            {
                Console.WriteLine("{0} > {1}", response.Sender, response.Text);
            }, x => x.WithTopic(gName));
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

            public IMessageMember Join(string name)
            {
                if (Members.Any(m => m.Name == name))
                {
                    throw new OperationCanceledException("Cannot initialize message member, There is already a member with this name");
                }

                MessageMember messageMember = new MessageMember(name, this);
                Members.Add(messageMember);
                if (MembersChanged != null)
                {
                    MembersChanged.Invoke(messageMember);
                }
                return messageMember;
            }

            public bool Exit(string name)
            {
                if (Members.Any(m => m.Name == name))
                {
                    var member = Members.FirstOrDefault(m => m.Name == name);
                    Members.Remove(member);
                    if (MembersChanged != null)
                    {
                        MembersChanged.Invoke(member);
                    }
                    return true;
                }
                
                return false;
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

            public IMessage Send(string text, IMessageMember target = null)
            {
                Message msg= new Message(text, this, target, Group);
                Group.Messages.Add(msg);

                MemberMessage publishMessage = new MemberMessage();
                publishMessage.PostedOn = msg.Time;
                publishMessage.GroupName = msg.Group?.Name;
                publishMessage.Sender = msg.Sender?.Name;
                publishMessage.Target = msg.Target?.Name;
                publishMessage.Text = msg.Text;

                Group.Bus.Publish(publishMessage, x => x.WithTopic(Group.Name));

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
