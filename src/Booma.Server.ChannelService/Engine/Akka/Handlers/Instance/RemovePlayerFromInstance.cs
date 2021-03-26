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

		private ICharacterInstanceSlotRepository SlotRepository { get; }

		public RemovePlayerFromInstance(IActorState<IActorMessageBroadcaster<WorldActorGroupType>> messageBroadcaster, 
			ICharacterInstanceSlotRepository slotRepository)
		{
			MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
			SlotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, LeaveWorldRequestMessage message, CancellationToken token = default)
		{
			//TODO: LOG THIS!
			//We don't know them??
			if (!await SlotRepository.ContainsEntitySlotAsync(message.Entity, token))
				return;

			var slot = await SlotRepository.RetrieveAsync(message.Entity, token);

			MessageBroadcaster.Data.RemoveFromGroup(WorldActorGroupType.Players, context.Sender);
			await SlotRepository.TryDeleteAsync(slot.Slot, token);

			//Who is the new leader?
			int leaderId = await SlotRepository.RetrieveLeaderIdAsync(token);

			//Once removed, for some reason server sends a sub60 command to all other players to indicate that the player has left.
			MessageBroadcaster.Data.BroadcastMessage(WorldActorGroupType.Players, new SendGamePacketMessage(new BlockOtherPlayerLeaveGameEventPayload((byte) slot.Slot, (byte) leaderId)));
		}
	}
}
