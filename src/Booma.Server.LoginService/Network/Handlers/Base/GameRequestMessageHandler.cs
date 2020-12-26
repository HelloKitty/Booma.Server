using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Special <see cref="BaseGameMessageHandler{TMessageType}"/> that implements request/response type semantics.
	/// </summary>
	/// <typeparam name="TMessageRequestType"></typeparam>
	/// <typeparam name="TMessageResponseType"></typeparam>
	public abstract class GameRequestMessageHandler<TMessageRequestType, TMessageResponseType> : BaseGameMessageHandler<TMessageRequestType> 
		where TMessageRequestType : PSOBBGamePacketPayloadClient
		where TMessageResponseType : PSOBBGamePacketPayloadServer
	{
		/// <summary>
		/// Indicates if the response sending should be awaited.
		/// Generally this should not be done unless the implementer
		/// absolutely knows that no other messages should be handled until this message is enqueued/sent.
		/// </summary>
		private bool AwaitResponseSend { get; }

		/// <summary>
		/// Request/response message handler.
		/// </summary>
		/// <param name="awaitResponseSend">Indicates if the response sending should be awaited.</param>
		protected GameRequestMessageHandler(bool awaitResponseSend = false)
		{
			AwaitResponseSend = awaitResponseSend;
		}

		/// <inheritdoc />
		public sealed override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, TMessageRequestType message, CancellationToken token = default)
		{
			//Concept here is to dispatch to the request handler and get a response to send.
			TMessageResponseType response = await HandleRequestAsync(context, message, token);

			if (AwaitResponseSend)
				await context.MessageService.SendMessageAsync(response, token);
			else
#pragma warning disable 4014
				context.MessageService.SendMessageAsync(response, token);
#pragma warning restore 4014
		}

		/// <summary>
		/// Similar to <see cref="HandleMessageAsync"/> but requires the implementer return an instance of the specified <typeparamref name="TMessageResponseType"/>.
		/// Which will be sent over the network.
		/// </summary>
		/// <param name="context">Message context.</param>
		/// <param name="message">Incoming message.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns></returns>
		protected abstract Task<TMessageResponseType> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, TMessageRequestType message, CancellationToken token = default);
	}
}
