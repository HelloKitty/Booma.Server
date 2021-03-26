using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.World
{
	[ActorMessageHandler(typeof(InstanceActor))]
	[ActorMessageHandler(typeof(LobbyActor))]
	public sealed class ForwardPacketMessageHandler : BaseActorMessageHandler<ForwardPacketMessage>
	{
		private ICharacterInstanceSlotRepository SlotRepository { get; }

		public ForwardPacketMessageHandler(ICharacterInstanceSlotRepository slotRepository)
		{
			SlotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, ForwardPacketMessage message, CancellationToken token = default)
		{
			//TODO: Logging if this isn't true.
			if (await SlotRepository.ContainsAsync(message.TargetSlot, token))
			{
				var slot = await SlotRepository.RetrieveAsync(message.TargetSlot, token);

				//TODO: Logging if this isn't true.
				if(slot.IsInitialized)
					slot.Actor.TellSelf(new SendGamePacketMessage(message.Packet));
			}
		}
	}
}
