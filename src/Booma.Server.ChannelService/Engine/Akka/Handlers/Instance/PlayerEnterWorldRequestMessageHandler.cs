using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.GameConfig;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class PlayerEnterWorldRequestMessageHandler : BaseActorMessageHandler<JoinWorldRequestMessage>
	{
		private IActorState<IActorMessageBroadcaster<WorldActorGroupType>> MessageBroadcaster { get; }

		private ICharacterInstanceSlotRepository SlotRepository { get; }

		private IBoomaGGDBFData Data { get; }

		public PlayerEnterWorldRequestMessageHandler(IActorState<IActorMessageBroadcaster<WorldActorGroupType>> messageBroadcaster, 
			ICharacterInstanceSlotRepository slotRepository, 
			IBoomaGGDBFData data)
		{
			MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
			SlotRepository = slotRepository ?? throw new ArgumentNullException(nameof(slotRepository));
			Data = data ?? throw new ArgumentNullException(nameof(data));
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
			return new CharacterJoinData(new PlayerInformationHeader((uint) guid.Identifier, slot, "Unknown"), CreateTestInventory(), new LobbyCharacterData(CreateStats(1, CharacterRace.Human, CharacterClass.HUmar), 0, 0, new CharacterProgress((uint) 0, 20), 0, String.Empty, 0, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Viridia, CharacterClass.HUmar,
				new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), "Unknown"));
		}

		private CharacterStats CreateStats(int level, CharacterRace race, CharacterClass @class)
		{
			var stats = new ushort[7];

			//Level 1 is represented as 0
			foreach(var entry in Data.CharacterStatDefault.CalculateBaseStats(level + 1, race, @class))
				stats[(int) entry.Key] = (ushort) entry.Value.Value;

			return new CharacterStats(stats);
		}

		private static CharacterInventoryData CreateTestInventory()
		{
			var saber = new InventoryItem(0x00010000, 0x1, 0, 0x8);
			saber.SetWeaponType(0x01);

			InventoryItem[] starterItems = new InventoryItem[] { saber };
			InventoryItem[] emptyItems = Enumerable.Repeat(new InventoryItem(), 30 - starterItems.Length).ToArray();

			return new CharacterInventoryData((byte) starterItems.Length, 0, 0, 1, starterItems.Concat(emptyItems).ToArray());
		}

		private async Task<PlayerInformationHeader[]> BuildPlayerInformationHeaders()
		{
			CharacterInstanceSlot[] initializedPlayers = (await SlotRepository.RetrieveInitializedAsync()).ToArray();

			var headers = new PlayerInformationHeader[initializedPlayers.Length];

			for (int i = 0; i < initializedPlayers.Length; i++)
				headers[i] = new PlayerInformationHeader((uint) initializedPlayers[i].Guid.Identifier, initializedPlayers[i].Slot, "Unknown");

			return headers;
		}
	}
}
