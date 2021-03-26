using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class PlayerJoinInstanceRequestMessageHandler : ActorRequestMessageHandler<PlayerJoinInstanceRequest, ResponseModel<string, PlayerJoinInstanceResponseCode>>
	{
		private IActorFactory<InstanceCharacterActor> InstanceCharacterFactory { get; }

		private ICharacterInstanceSlotRepository SlotRepository { get; }

		public PlayerJoinInstanceRequestMessageHandler(IActorFactory<InstanceCharacterActor> instanceCharacterFactory, 
			ICharacterInstanceSlotRepository slotRepository)
		{
			InstanceCharacterFactory = instanceCharacterFactory ?? throw new ArgumentNullException(nameof(instanceCharacterFactory));
			SlotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
		}

		/// <inheritdoc />
		protected override async Task<ResponseModel<string, PlayerJoinInstanceResponseCode>> HandleRequestAsync(EntityActorMessageContext context, PlayerJoinInstanceRequest message, CancellationToken token = default)
		{
			var character = InstanceCharacterFactory.Create(new ActorCreationContext(context.ActorContext));

			if (!await SlotRepository.HasAvailableSlotAsync(token))
				return new ResponseModel<string, PlayerJoinInstanceResponseCode>(PlayerJoinInstanceResponseCode.InstanceFull);

			int slot = await SlotRepository.FirstAvailableSlotAsync(token);
			if (!await SlotRepository.TryCreateAsync(new CharacterInstanceSlot(slot, character, message.PlayerGuid), token))
				return new ResponseModel<string, PlayerJoinInstanceResponseCode>(PlayerJoinInstanceResponseCode.GeneralServerError);

			return new ResponseModel<string, PlayerJoinInstanceResponseCode>(character.Actor.Path.ToString());
		}
	}
}
