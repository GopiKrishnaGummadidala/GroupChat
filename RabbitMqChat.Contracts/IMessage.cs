using System;

namespace RabbitMqChat.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IMessage : IDisposable
    {
        /// <summary>
        ///  When empty this message is not any more part of a <see cref="IMessageGroup"/> and has probably be deleted.
        /// </summary>
        IMessageGroup Group { get; }

        /// <summary>
        ///  The sender of the <see cref="IMessage"/>
        /// </summary>
        IMessageMember Sender { get; }

        /// <summary>
        ///  The optional target of the <see cref="IMessage"/>
        /// </summary>
        IMessageMember Target { get; }

        /// <summary>
        ///  The text.
        /// </summary>
        string Text { get; }

        /// <summary>
        ///  Time of sent.
        /// </summary>
        DateTime Time { get; }
    }
}
