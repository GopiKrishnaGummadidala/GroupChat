﻿using System;
using System.Linq;
using EasyNetQ;
using RabbitMqChat.Contracts;
using IMessage = RabbitMqChat.Contracts.IMessage;

namespace RabbitMqChat
{
    class Program
    {
        static void Main(string[] args)
        {
            var uName = Ask("Please enter your nick name: ");
            var gName = Ask("Please enter your Group name: ");

            using (var _bus = RabbitHutch.CreateBus("host=localhost", x => x.Register<IEasyNetQLogger>(_ => new EmptyLogger())))
            {
                MessageGroupFactory messageGroupFactory = new MessageGroupFactory(_bus);
                var group = messageGroupFactory.Create(gName, uName);
                if (group.Name != gName ||
                    !group.Members.Any())
                {

                }

                group.MembersChanged += Group_MembersChanged;
                group.MessageChanged += Group_MessageChanged;

                var me = group.Join(uName);
                if (me.Name != uName ||
                    me.Group != group ||
                    !group.Members.Contains(me))
                {

                }

                while (true)
                {
                    Console.Write("> ");
                    var msg = Console.ReadLine();

                    if (msg.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                    {
                        SendExitMessage(uName);
                        break;
                    }
                    else if (msg.StartsWith(":pub"))
                        PublishManyMessages(uName);
                    else if (msg.StartsWith(":cmd"))
                        SendCommand(uName);
                    else
                    {
                        var message = me.Send(msg,null);
                    }
                }
            }
        }

        private static void Group_MessageChanged(IMessage obj)
        {

        }

        private static void Group_MembersChanged(IMessageMember obj)
        {

        }

        private static string Ask(string prompt)
        {
            string result = null;
            while (string.IsNullOrEmpty(result))
            {
                Console.Write(prompt);
                result = Console.ReadLine();
            }
            return result;
        }

        private static void SendExitMessage(string user)
        {
            //_bus.Publish<Leaved>(new Leaved() { LeftOn = DateTime.Now, User = user });
        }

        private static void SendMessage(string user, string msg)
        {
            //_bus.Publish<Message>(new Message { PostedOn = DateTime.Now, User = user, Text = msg });
        }

        private static void PublishManyMessages(string user)
        {
            for (var a = 0; a < 100; a++)
                SendMessage(user, ":delay Sample message " + a);
        }

        private static void SendCommand(string user)
        {
            Console.WriteLine("Randomly asking for nickname");
        }

    }

    public class EmptyLogger : IEasyNetQLogger
    {
        public void DebugWrite(string format, params object[] args)
        {
        }

        public void InfoWrite(string format, params object[] args)
        {
        }

        public void ErrorWrite(string format, params object[] args)
        {
        }

        public void ErrorWrite(Exception exception)
        {
        }
    }
}
/// adding a new feature