using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.InstanceCharacter
{
	[ActorMessageHandler(typeof(InstanceCharacterActor))]
	public sealed class PostInitializeActorMessageHandler : BaseActorMessageHandler<PostInitializeActorMessage>
	{
		private IActorState<NetworkEntityGuid> ActorGuid { get; }

		public PostInitializeActorMessageHandler(IActorState<NetworkEntityGuid> actorGuid)
		{
			ActorGuid = actorGuid ?? throw new ArgumentNullException(nameof(actorGuid));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, PostInitializeActorMessage message, CancellationToken token = default)
		{
			//Tell the parent instance actor that we want to join the world now
			context.ActorContext.Parent.TellEntity(new JoinWorldRequestMessage(ActorGuid.Data));
		}
	}
}
