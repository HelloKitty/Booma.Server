using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Lobby
{
	[ActorMessageHandler(typeof(LobbyActor))]
	public sealed class RemovePlayerFromLobby : BaseActorMessageHandler<LeaveWorldRequestMessage>
	{
		private ICharacterLobbySlotRepository LobbySlotRepository { get; }

		private IActorState<IActorMessageBroadcaster<LobbyActorGroupType>> LobbyMessageBroadcaster { get; }

		public RemovePlayerFromLobby(ICharacterLobbySlotRepository lobbySlotRepository, 
			IActorState<IActorMessageBroadcaster<LobbyActorGroupType>> lobbyMessageBroadcaster)
		{
			LobbySlotRepository = lobbySlotRepository ?? throw new ArgumentNullException(nameof(lobbySlotRepository));
			LobbyMessageBroadcaster = lobbyMessageBroadcaster ?? throw new ArgumentNullException(nameof(lobbyMessageBroadcaster));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, LeaveWorldRequestMessage message, CancellationToken token = new CancellationToken())
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
				LobbyMessageBroadcaster.Data.RemoveFromGroup(LobbyActorGroupType.Players, slot.Actor);

				context.ActorContext
					.ActorSelection("*")
					.Tell(new PlayerLeftWorldEventMessage(slot.CharacterData.EntityGuid, slot.Slot));
			}

			await LobbySlotRepository.TryDeleteAsync(slot.Slot, token);
		}
	}
}
