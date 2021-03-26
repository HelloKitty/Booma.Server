using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Character
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	[ActorMessageHandler(typeof(InstanceCharacterActor))]
	public sealed class ForwardPacketMessageHandler : BaseActorMessageHandler<ForwardPacketMessage>
	{
		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, ForwardPacketMessage message, CancellationToken token = default)
		{
			//TODO: we need to do some validation?
			context.ActorContext
				.Parent.Tell(message, context.Entity);
		}
	}
}
