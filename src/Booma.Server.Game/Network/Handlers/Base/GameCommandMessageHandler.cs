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
	/// Special <see cref="GameMessageHandler{TMessageType}"/> that implements command semantics (empty payload).
	/// </summary>
	/// <typeparam name="TMessageRequestType"></typeparam>
	public abstract class GameCommandMessageHandler<TMessageRequestType> : GameMessageHandler<TMessageRequestType> 
		where TMessageRequestType : PSOBBGamePacketPayloadClient
	{
		/// <inheritdoc />
		public sealed override Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, TMessageRequestType message, CancellationToken token = default)
		{
			return HandleCommandAsync(context, token);
		}

		/// <summary>
		/// Similar to <see cref="HandleMessageAsync"/> but doesn't provide the message itself as it is an empty message.
		/// </summary>
		/// <param name="context">Message context.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns></returns>
		protected abstract Task HandleCommandAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CancellationToken token = default);
	}
}
