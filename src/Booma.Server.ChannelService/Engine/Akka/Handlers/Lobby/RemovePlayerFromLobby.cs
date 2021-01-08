using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Lobby
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	public sealed class RemovePlayerFromLobby : BaseActorMessageHandler<LeaveLobbyRequestMessage>
	{
		private ICharacterLobbySlotRepository LobbySlotRepository { get; }

		public RemovePlayerFromLobby(ICharacterLobbySlotRepository lobbySlotRepository)
		{
			LobbySlotRepository = lobbySlotRepository ?? throw new ArgumentNullException(nameof(lobbySlotRepository));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, LeaveLobbyRequestMessage message, CancellationToken token = new CancellationToken())
		{
			if (!await LobbySlotRepository.ContainsEntitySlotAsync(message.Entity, token))
			{
				//TODO: We should log, that we don't know the entity asking to leave the lobby!!
				return;
			}

			//We know the entity, now we must decide if we need to let other lobby players know.
			CharacterLobbySlot slot = await LobbySlotRepository.RetrieveAsync(message.Entity, token);

			//Must let all players know a client is leaving
			//Any other actor in the lobby may be interested in this too so we can
			//broadcast to all.
			if (slot.IsInitialized)
			{
				context.ActorContext
					.ActorSelection("*")
					.Tell(new PlayerLeftLobbyEventMessage(slot.CharacterData.EntityGuid, slot.Slot));
			}

			await LobbySlotRepository.TryDeleteAsync(slot.Slot, token);
		}
	}
}
