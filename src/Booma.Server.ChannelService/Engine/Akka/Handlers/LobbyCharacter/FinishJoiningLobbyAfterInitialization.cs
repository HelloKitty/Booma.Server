using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.LobbyCharacter
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	public sealed class FinishJoiningLobbyAfterInitialization : BaseActorMessageHandler<PreJoinInitializationFinishedMessage>
	{
		private IActorState<NetworkEntityGuid> GuidState { get; }

		public FinishJoiningLobbyAfterInitialization(IActorState<NetworkEntityGuid> guidState)
		{
			GuidState = guidState ?? throw new ArgumentNullException(nameof(guidState));
		}

		public override Task HandleMessageAsync(EntityActorMessageContext context, PreJoinInitializationFinishedMessage message, CancellationToken token = default)
		{
			//Tell lobby (parent) that we want to finally join
			context.ActorContext.Parent
				.Tell(new JoinLobbyRequestMessage(GuidState.Data));

			return Task.CompletedTask;
		}
	}
}
