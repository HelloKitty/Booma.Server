using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Lobby
{
	[ActorMessageHandler(typeof(LobbyActor))]
	public sealed class BroadcastGamePacketMessageHandler : BaseActorMessageHandler<BroadcastGamePacketMessage>
	{
		private ICharacterLobbySlotRepository LobbyCharacterRepository { get; }

		public BroadcastGamePacketMessageHandler(ICharacterLobbySlotRepository lobbyCharacterRepository)
		{
			LobbyCharacterRepository = lobbyCharacterRepository ?? throw new ArgumentNullException(nameof(lobbyCharacterRepository));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, BroadcastGamePacketMessage message, CancellationToken token = new CancellationToken())
		{
			//TODO: make broadcasting API
			foreach(var lobbyPlayer in await LobbyCharacterRepository.RetrieveInitializedAsync(token))
			{
				if(lobbyPlayer.CharacterData.EntityGuid != message.Broadcaster)
					lobbyPlayer.EntityActor.Actor.TellEntity(new SendGamePacketMessage(message.Packet));
			}
		}
	}
}
