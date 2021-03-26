using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class PlayerEnterWorldRequestMessageHandler : BaseActorMessageHandler<JoinWorldRequestMessage>
	{
		private IActorState<IActorMessageBroadcaster<WorldActorGroupType>> MessageBroadcaster { get; }

		private ICharacterInstanceSlotRepository SlotRepository { get; }

		public PlayerEnterWorldRequestMessageHandler(IActorState<IActorMessageBroadcaster<WorldActorGroupType>> messageBroadcaster, 
			ICharacterInstanceSlotRepository slotRepository)
		{
			MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
			SlotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, JoinWorldRequestMessage message, CancellationToken token = default)
		{
			if (!await SlotRepository.ContainsEntitySlotAsync(message.Entity, token))
			{
				message.Answer(context.Sender, WorldJoinResponseCode.GeneralServerError);
				return;
			}

			var slot = await SlotRepository.RetrieveAsync(message.Entity, token);

			//We consider the LEADER id to be the lowest initialized player.
			int leaderId = await SlotRepository.RetrieveLeaderIdAsync(token);

			//TODO: We need to go through the COMPLEX process of creating a GameLobby actor in AKKA and transfering our session's
			//actor from the current lobby (see lobby change/switch code) and create a new Player actor in the GameLobby
			context.Sender.TellEntity(new SendGamePacketMessage(new BlockGameJoinEventPayload((byte) slot.Slot, (byte) leaderId, new GameSettings(DifficultyType.Normal, false, 0, SectionId.Viridia, false, 0, EpisodeType.EpisodeI, false), await BuildPlayerInformationHeaders())));

			//Let all other players know about the joiner before we join the broadcast group ourselves
			MessageBroadcaster.Data.BroadcastMessage(WorldActorGroupType.Players, new SendGamePacketMessage(new BlockGamePlayerJoinedEventPayload((byte) slot.Slot, (byte) leaderId, CreateJoinData(message.Entity, (byte) slot.Slot))));

			MessageBroadcaster.Data.AddToGroup(WorldActorGroupType.Players, context.Sender);
			message.Answer(context.Sender, WorldJoinResponseCode.Success);
			slot.IsInitialized = true;
		}

		private CharacterJoinData CreateJoinData(NetworkEntityGuid guid, byte slot)
		{
			return new CharacterJoinData(new PlayerInformationHeader((uint) guid.Identifier, slot, "Unknown"), new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), new LobbyCharacterData(CreateEmptyStats(), 0, 0, new CharacterProgress((uint) 0, 1), 0, String.Empty, 0, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Viridia, CharacterClass.HUmar,
				new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), "Unknown"));
		}

		private async Task<PlayerInformationHeader[]> BuildPlayerInformationHeaders()
		{
			CharacterInstanceSlot[] initializedPlayers = (await SlotRepository.RetrieveInitializedAsync()).ToArray();

			var headers = new PlayerInformationHeader[initializedPlayers.Length];

			for (int i = 0; i < initializedPlayers.Length; i++)
				headers[i] = new PlayerInformationHeader((uint) initializedPlayers[i].Guid.Identifier, initializedPlayers[i].Slot, "Unknown");

			return headers;
		}

		private static CharacterStats CreateEmptyStats()
		{
			return new CharacterStats(Enumerable.Repeat((ushort)1, 7).ToArray());
		}
	}
}
