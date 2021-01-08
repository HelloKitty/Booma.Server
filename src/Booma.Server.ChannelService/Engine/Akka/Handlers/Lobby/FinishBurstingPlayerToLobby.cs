using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(GameLobbyActor))]
	public sealed class FinishBurstingPlayerToLobby : BaseActorMessageHandler<JoinLobbyRequestMessage>
	{
		private ICharacterLobbySlotRepository LobbyCharacterRepository { get; }

		public FinishBurstingPlayerToLobby(ICharacterLobbySlotRepository lobbyCharacterRepository)
		{
			LobbyCharacterRepository = lobbyCharacterRepository ?? throw new ArgumentNullException(nameof(lobbyCharacterRepository));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, JoinLobbyRequestMessage message, CancellationToken token = default)
		{
			//If we don't KNOW this entity, maybe it's been kicked or something, then don't finish joining them.
			if (!await LobbyCharacterRepository.ContainsEntitySlotAsync(message.Entity, token))
			{
				message.Answer(context.Sender, LobbyJoinResponseCode.GeneralServerError);
				return;
			}

			//We just mark it as initialized which indicates to the Lobby that it's ready for NORMAL lobby stuff.
			CharacterLobbySlot slot = await LobbyCharacterRepository.RetrieveAsync(message.Entity, token);
			slot.IsInitialized = true;
			message.Answer(context.Sender, LobbyJoinResponseCode.Success);


			var characterJoinDatas = await BuildCharacterJoinData();
			//TODO: Input accurate block/lobby id.
			//Send the initial lobby join packet to the actor
			context.Sender.Tell(new SendGamePacketMessage(new BlockLobbyJoinEventPayload((byte) slot.Slot, 0, 0, 1, 0, characterJoinDatas)));

			//Now we should broadcast to all lobby clients that
			//a player has joined the lobby.
			CharacterJoinData newJoinData = characterJoinDatas.First(d => d.PlayerHeader.ClientId == slot.Slot);
			var broadcastPacket = new BlockOtherPlayerJoinedLobbyEventPayload();

			//TODO: make broadcasting API
			foreach (var lobbyPlayer in await LobbyCharacterRepository.RetrieveInitializedAsync(token))
			{
				if (lobbyPlayer.Slot != newJoinData.PlayerHeader.ClientId)
					lobbyPlayer.Actor.Tell(new SendGamePacketMessage(broadcastPacket));
			}
		}

		private async Task<CharacterJoinData[]> BuildCharacterJoinData()
		{
			return (await LobbyCharacterRepository
					.RetrieveAllAsync(CancellationToken.None))
				.Where(slot => slot != null && slot.IsInitialized) //only send initialized data, don't send mid-bursting clients.
				.Select(slot => new CharacterJoinData(new PlayerInformationHeader(slot.CharacterData.GuildCard.GuildCard, slot.Slot, slot.CharacterData.Name), slot.CharacterData.Inventory, BuildLobbyData(slot.CharacterData)))
				.ToArray();
		}
		
		//TODO: Use object mapper.
		private LobbyCharacterData BuildLobbyData(InitialCharacterDataSnapshot data)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));

			return new LobbyCharacterData(data.Stats, 0, 0, data.Progress, 0, data.GuildCard.GuildCard.ToString(), 0, data.SpecialCustom, data.GuildCard.SectionId, data.GuildCard.ClassType, data.Version, data.Customization, data.Name);
		}
	}
}
