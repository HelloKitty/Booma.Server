using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Lobby
{
	[ActorMessageHandler(typeof(LobbyActor))]
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class ActorBroadcasterPostInitializeActorMessageHandler : BaseActorMessageHandler<PostInitializeActorMessage>
	{
		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, PostInitializeActorMessage message, CancellationToken token = default)
		{
			//This is enable the broadcast group IActorMessageBroadcaster<WorldActorGroupType>
			context.Entity.InitializeState<IActorMessageBroadcaster<WorldActorGroupType>>(new DefaultGenericActorMessageBroadcaster<WorldActorGroupType>(context.ActorContext));
		}
	}
}
