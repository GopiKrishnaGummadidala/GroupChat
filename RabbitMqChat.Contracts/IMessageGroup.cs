using System;
using System.Collections.Generic;

namespace RabbitMqChat.Contracts
{

    /// <summary>
    /// 
    /// </summary>
    public interface IMessageGroup : IDisposable
    {
        /// <summary>
        ///  A unique name of the group
        /// </summary>
        string Name { get; }

        /// <summary>
        ///  The members that are currently here.
        /// </summary>
        IList<IMessageMember> Members { get; }

        /// <summary>
        ///  Sent on any change in <see cref="Members"/>.
        /// </summary>
        event Action<IMessageMember> MembersChanged;

        /// <summary>
        ///  Join this <see cref="IMessageGroup"/> with a unique <paramref name="name"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>
        ///  a <see cref="IMessageMember"/> if possible.
        /// </returns>
        IMessageMember Join(string name);

        /// <summary>
        ///  The messages that are here. Ordered by <see cref="IMessage.Time"/> and only
        ///  those where <see cref="IMessage.Sender"/> is part of <see cref="Members"/>.
        /// </summary>
        IList<IMessage> Messages { get; }

        /// <summary>
        ///  Is triggered on any change in <see cref="Messages"/>.
        /// </summary>
        event Action<IMessage> MessageChanged;

    }
}
