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

        public IList<IMessageGroup> GroupList { get; private set; }

        public MessageGroupFactory(IBus bus)
        {
            Bus = bus;
            GroupList = new List<IMessageGroup>();
        }

        public IMessageGroup Create(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                IMessageGroup existedGroup = CheckForGroup(name);
                if (existedGroup != null)
                {
                    // TO DO: Bus Communication
                    // PublishGroupName(name);
                    // Console.WriteLine("Group is created with Name : {0}", name);
                    // new MessageGroup(Bus, name);
                    return existedGroup;
                }
                else
                {
                    // To Do: Clone the existing MesssageGroup Object
                    //return  new MessageGroup(Bus, name);

                    MessageGroup msgGroup = new MessageGroup(Bus, name);
                    GroupList.Add(msgGroup);

                    return msgGroup;
                }
            }

            throw new ArgumentNullException("Cannot initialize group, without Name");
        }

        private IMessageGroup CheckForGroup(string gName)
        {
            try
            {
                return GroupList.FirstOrDefault(g => g.Name == gName);
                // TO DO: Bus Communication
                //var response = Bus.Request<GroupRequest, GroupResponse>(new GroupRequest { });
                //if (gName.Equals(response.Name))
                //{
                //    Console.WriteLine("Group is available with Name : {0}", response.Name);
                //}
                //return gName.Equals(response.Name);
            }
            catch (Exception e)
            {
                return null;
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

            public void SendMessageChanged(IMessage message)
            {
                if (MessageChanged != null)
                {
                    MessageChanged.Invoke(message);
                }
            }

            public void SendMemberChanged(IMessageMember member)
            {
                if (MembersChanged != null)
                {
                    MembersChanged.Invoke(member);
                }
            }

            public MessageGroup(IBus bus, string name)
            {
                Bus = bus;
                Name = name;
                Members = new List<IMessageMember>();
                Messages = new List<IMessage>();
            }

            public IMessageMember Join(string name)
            {
                if (!string.IsNullOrEmpty(name))
                {
                    if (Members.Any(m => m.Name == name))
                    {
                        throw new ArgumentOutOfRangeException("Cannot initialize message member, There is already a member with this name");
                    }

                    MessageMember messageMember = new MessageMember(name, this);
                    Members.Add(messageMember);

                    // TO DO: Bus Communication
                    //SubscribeMessage(Name, name);
                    //SubscribeMemberJoin(Name, name);
                    //SendMemberJoin(Name, name);
                    //SubscribeMemberExit(Name, name);

                    if (MembersChanged != null)
                    {
                        MembersChanged.Invoke(messageMember);
                    }
                    return messageMember;
                }
                throw new ArgumentNullException("Cannot initialize message member, without Name");
            }

            private void SendMemberJoin(string gName, string uName)
            {
                Bus.Publish(new MemberJoin { Name = uName, GroupName = gName }, x => x.WithTopic(gName));
            }

            private void SubscribeMemberJoin(string gName, string uName)
            {
                Bus.Subscribe<MemberJoin>(uName, response =>
                {
                    //Member member = new Member();
                    //member.Name = response.Name;
                    //member.GroupName = response.GroupName;
                    //MemberList.Add(member);

                    //Console.WriteLine("Member : {0} joined to this Group : {1}", response.Name, response.GroupName);
                    //Console.WriteLine("Member Count: {0}", MemberList.Count);

                }, x => x.WithTopic(gName));
            }

            private void SubscribeMessage(string gName, string uName)
            {
                Bus.Subscribe<MemberMessage>(uName, response =>
                {
                    Console.WriteLine("{0} > {1}", response.Sender, response.Text);
                }, x => x.WithTopic(gName));

                //Messaging to a direct member among any group
                //Bus.Subscribe<MemberMessage>(uName, response =>
                //{
                //    Console.WriteLine("{0} > {1}", response.Sender, response.Text);
                //}, x => x.WithTopic(uName));
            }

            private void SubscribeMemberExit(string gName, string uName)
            {
                Bus.Subscribe<MemberLeave>(uName, response =>
                {
                    //Console.WriteLine("Member : {0} left from this Group : {1}", response.Name, response.GroupName);
                    //if (MemberList.Any(m => m.Name == response.Name))
                    //{
                    //    var member = MemberList.FirstOrDefault(m => m.Name == response.Name);
                    //    MemberList.Remove(member);
                    //}

                    //Console.WriteLine("Member Count: {0}", MemberList.Count);
                }, x => x.WithTopic(gName));
            }

            private void SendMasterMemberList(string gName)
            {
                //Bus.Publish(new MasterMemberList { Members = MemberList }, x => x.WithTopic(gName));
            }

            private void SubscribeMasterMemberList(string gName, string uName)
            {
                Bus.Subscribe<MasterMemberList>(uName, response =>
                {
                    //MemberList = response.Members;
                    //Console.WriteLine("Member List Count: {0}", response.Members.Count);
                }, x => x.WithTopic(gName));
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
                foreach (var member in Members.ToList())
                {
                    member.Dispose();
                }
            }
        }

        private class MessageMember : IMessageMember
        {
            public string Name { get; private set; }

            public IMessageGroup Group { get; private set; }

            private MessageGroup MessageGroup { get; set; }

            public MessageMember(string name, IMessageGroup group)
            {
                Name = name;
                Group = group;
                MessageGroup = group as MessageGroup;
            }

            public void Dispose()
            {
                if (Group != null)
                {
                    List<IMessage> msgs = Group.Messages.Where(m => m.Sender == this).ToList();

                    foreach (var message in msgs)
                    {
                        message.Dispose();
                    }

                    if (Group.Members.Remove(this))
                    {
                        var messageGroup = MessageGroup;
                        MessageGroup = null;
                        Group = null;
                        messageGroup?.SendMemberChanged(this);
                    }

                    // TO DO: Bus Communication
                    //SendMemberExit(MessageGroup.Name, Name);

                }
            }

            private void SendMemberExit(string gName, string uName)
            {
                MessageGroup.Bus.Publish(new MemberLeave { Name = uName, GroupName = gName }, x => x.WithTopic(gName));
                DeleteQueue("RabbitMqChat.Models.MemberJoin:RabbitMqChat.Models_" + uName, false);
                DeleteQueue("RabbitMqChat.Models.MemberLeave:RabbitMqChat.Models_" + uName, false);
                DeleteQueue("RabbitMqChat.Models.MemberMessage:RabbitMqChat.Models_" + uName, false);
            }

            private void DeleteQueue(string qName, bool isExclusive)
            {
                try
                {
                    IQueue queue = new Queue(qName, isExclusive);
                    MessageGroup.Bus.Advanced.QueueDelete(queue);
                }
                catch (Exception e)
                {
                    //To Do: Exception Logging
                }
            }

            public IMessage Send(string text, IMessageMember target = null)
            {
                Message msg = null;
                try
                {
                    if (Group != null && target == null || target.Group == Group)
                    {
                        msg = new Message(text, this, target, Group);
                        Group.Messages.Add(msg);
                        MessageGroup.SendMessageChanged(msg);
                        return msg;
                    }

                    //MemberMessage publishMessage = new MemberMessage();
                    //publishMessage.PostedOn = msg.Time;
                    //publishMessage.GroupName = msg.Group?.Name;
                    //publishMessage.Sender = msg.Sender?.Name;
                    //publishMessage.Target = msg.Target?.Name;
                    //publishMessage.Text = msg.Text;

                    //if (target != null)
                    //{
                    //    MessageGroup.Bus.Publish(publishMessage, x => x.WithTopic(target));
                    //}
                    //else
                    //{
                    //    MessageGroup.Bus.Publish(publishMessage, x => x.WithTopic(Group.Name));
                    //}
                }
                catch (Exception ex)
                {
                    // To Do: Exception Logging
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
                Time = DateTime.UtcNow;
            }

            public void Dispose()
            {
                if (Group != null)
                {
                    if (Group.Messages.Remove(this))
                    {
                        var group = Group as MessageGroup;
                        Group = null;
                        group?.SendMessageChanged(this);
                    }

                }
            }
        }
    }
}
