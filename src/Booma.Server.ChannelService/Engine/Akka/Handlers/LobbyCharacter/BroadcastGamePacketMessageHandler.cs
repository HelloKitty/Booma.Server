using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.LobbyCharacter
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	public sealed class BroadcastGamePacketMessageHandler : BaseActorMessageHandler<BroadcastGamePacketMessage>
	{
		public override async Task HandleMessageAsync(EntityActorMessageContext context, BroadcastGamePacketMessage message, CancellationToken token = new CancellationToken())
		{
			//TODO: we need to do some validation?
			context.ActorContext
				.Parent.Tell(message, context.Entity);
		}
	}
}
