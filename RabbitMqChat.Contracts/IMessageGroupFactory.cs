using System;

namespace RabbitMqChat.Contracts
{
    /// <summary>
    ///  Factory for <see cref="IMessageGroup"/>s.
    /// </summary>
    public interface IMessageGroupFactory: IDisposable
    {
        /// <summary>
        ///  Create a new <see cref="IMessageGroup"/> with a unique <paramref name="gName"/>.
        /// </summary>
        /// <param name="gName"></param>
        /// <param name="uName"></param>
        /// <returns>
        /// </returns>
        IMessageGroup Create(string gName, string uName);
    }
}
