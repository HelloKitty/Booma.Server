using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.LobbyPlayer
{
	public sealed class LeaveLobbyOnActorOwnerDisposed : BaseActorMessageHandler<ActorOwnerDisposedMessage>
	{
		private IActorState<NetworkEntityGuid> GuidState { get; }

		public LeaveLobbyOnActorOwnerDisposed(IActorState<NetworkEntityGuid> guidState)
		{
			GuidState = guidState ?? throw new ArgumentNullException(nameof(guidState));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, ActorOwnerDisposedMessage message, CancellationToken token = new CancellationToken())
		{
			//Owner/client/session has disposed of this player actor in some fashion
			//so we should tell the lobby (parent) that we are leaving
			context.ActorContext.Parent
				.Tell(new LeaveLobbyRequestMessage(GuidState.Data));

			//Stops us from handling ANY more messages.
			context.ActorContext.Stop(context.ActorContext.Self);
		}
	}
}
