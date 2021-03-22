using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Lobby
{
	[ActorMessageHandler(typeof(LobbyActor))]
	public sealed class LobbyPostInitializeActorMessageHandler : BaseActorMessageHandler<PostInitializeActorMessage>
	{
		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, PostInitializeActorMessage message, CancellationToken token = default)
		{
			//This is enable the broadcast group IActorMessageBroadcaster<LobbyActorGroupType>
			context.Entity.InitializeState<IActorMessageBroadcaster<LobbyActorGroupType>>(new DefaultGenericActorMessageBroadcaster<LobbyActorGroupType>(context.ActorContext));
		}
	}
}
