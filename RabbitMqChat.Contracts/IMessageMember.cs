using EasyNetQ;
using System;

namespace RabbitMqChat.Contracts
{
    public interface IMessageMember : IDisposable
    {
        /// <summary>
        ///  The unique name of the member.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///  The group the <see cref="IMessageMember"/> is in. Can be <c>null</c> if disposed or out.
        /// </summary>
        IMessageGroup Group { get; }

        /// <summary>
        ///  Send some text message.
        /// </summary>
        /// <param name="text">
        ///  used to fill <see cref="IMessage.Text"/>.
        /// </param>
        /// <param name="target">
        ///  a optional target of communication. This just fills <see cref="IMessage.Target"/> but is still sent everywhere.
        /// </param>
        /// <returns>
        ///  the message sent or throws an exception if e.g. we are not in a <see cref="Group"/>.
        /// </returns>
        IMessage Send(string text, string target = null);

    }
}
