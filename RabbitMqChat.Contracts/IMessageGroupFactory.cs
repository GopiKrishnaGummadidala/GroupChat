using System;
using System.Collections.Generic;

namespace RabbitMqChat.Contracts
{
    /// <summary>
    ///  Factory for <see cref="IMessageGroup"/>s.
    ///  If a factory is disposed all groups it has <see cref="Create(string)"/>ed are disposed.
    /// </summary>
    public interface IMessageGroupFactory : IDisposable
    {

        /// <summary>
        ///  The list of groups that are currently here.
        /// </summary>
        IList<IMessageGroup> GroupList { get; }

        /// <summary>
        ///  Create a new <see cref="IMessageGroup"/> with a unique <paramref name="name"/>.
        ///  If a group with that <paramref name="name"/> already exists, then this one returned otherwise it creates a new one.
        /// </summary>
        /// <param name="name">
        ///  the name of the group
        /// </param>
        /// <returns>
        ///  a <see cref="IMessageGroup"/> where <see cref="IMessageGroup.Name"/> is <paramref name="name"/>.
        /// </returns>
        IMessageGroup Create(string name);
    }
}
