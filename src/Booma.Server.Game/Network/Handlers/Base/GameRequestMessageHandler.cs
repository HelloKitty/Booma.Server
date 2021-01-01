using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Special <see cref="GameMessageHandler{TMessageType}"/> that implements request/response type semantics.
	/// </summary>
	/// <typeparam name="TMessageRequestType"></typeparam>
	/// <typeparam name="TMessageResponseType"></typeparam>
	public abstract class GameRequestMessageHandler<TMessageRequestType, TMessageResponseType> : GameMessageHandler<TMessageRequestType> 
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

			//Support returning nothing
			if (response == default)
				return;

			SendResult result = SendResult.Error;
			try
			{
				if (AwaitResponseSend)
					result = await context.MessageService.SendMessageAsync(response, token);
				else
#pragma warning disable 4014
				{
					context.MessageService.SendMessageAsync(response, token);
					result = SendResult.Enqueued; //TODO: This is a hacky guess.
				}
#pragma warning restore 4014
			}
			catch (Exception e)
			{
				result = SendResult.Error;
				throw;
			}
			finally
			{
				await OnResponseMessageSendAsync(context, message, response, result);
			}
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

		/// <summary>
		/// Implementer can override this method as a callback/event for when the <see cref="response"/> has been sent to the session.
		/// Called after <see cref="HandleMessageAsync"/>.
		/// 
		/// Implementers should not await directly within this message as it blocks the request pipeline unless they are absolutely sure they want this to happen.
		/// </summary>
		/// <param name="context">The message context.</param>
		/// <param name="request">The original request message.</param>
		/// <param name="response">The response message sent.</param>
		/// <param name="sendResult">The sent result of the response message.</param>
		/// <returns></returns>
		protected virtual Task OnResponseMessageSendAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, TMessageRequestType request, TMessageResponseType response, SendResult sendResult)
		{
			return Task.CompletedTask;
		}
	}
}
