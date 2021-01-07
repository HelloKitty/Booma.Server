using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using MEAKKA;

namespace Booma
{
	public static class IActorRequestMessageExtensions
	{
		/// <summary>
		/// Answers a <see cref="IActorRequestMessage{TResponseMessageType}"/> to the <see cref="IActorRef"/> sender
		/// with the expected <typeparamref name="TResponseMessageType"/>.
		/// </summary>
		/// <typeparam name="TResponseMessageType"></typeparam>
		/// <param name="request">The request to response to.</param>
		/// <param name="sender">The sender (target) to answer.</param>
		/// <param name="response">The response message.</param>
		public static void Answer<TResponseMessageType>(this IActorRequestMessage<TResponseMessageType> request, IActorRef sender, TResponseMessageType response)
		{
			if (request == null) throw new ArgumentNullException(nameof(request));
			if (sender == null) throw new ArgumentNullException(nameof(sender));

			sender.Tell(response);
		}
	}

	/// <summary>
	/// Interface that indicates that a type expects a specific <typeparamref name="TResponseMessageType"/> message
	/// type as the response message.
	/// </summary>
	/// <typeparam name="TResponseMessageType">The response message type.</typeparam>
	public interface IActorRequestMessage<TResponseMessageType>
	{

	}
}
