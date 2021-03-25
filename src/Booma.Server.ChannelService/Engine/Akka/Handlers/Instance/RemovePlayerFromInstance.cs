using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Instance
{
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class RemovePlayerFromInstance : BaseActorMessageHandler<LeaveWorldRequestMessage>
	{
		private IActorState<IActorMessageBroadcaster<WorldActorGroupType>> MessageBroadcaster { get; }

		public RemovePlayerFromInstance(IActorState<IActorMessageBroadcaster<WorldActorGroupType>> messageBroadcaster)
		{
			MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, LeaveWorldRequestMessage message, CancellationToken token = default)
		{
			MessageBroadcaster.Data.RemoveFromGroup(WorldActorGroupType.Players, context.Sender);

			//Once removed, for some reason server sends a sub60 command to all other players to indicate that the player has left.
			MessageBroadcaster.Data.BroadcastMessage(WorldActorGroupType.Players, new SendGamePacketMessage(new BlockOtherPlayerLeaveGameEventPayload()));
		}
	}
}
