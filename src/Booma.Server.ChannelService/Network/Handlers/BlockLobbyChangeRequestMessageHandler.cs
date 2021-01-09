using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace Booma
{
	public sealed class BlockLobbyChangeRequestMessageHandler : GameMessageHandler<BlockLobbyChangeRequestPayload>
	{
		private ICharacterActorReferenceContainer ActorContainer { get; }

		private ILobbyEntryService LobbyEntryService { get; }

		public BlockLobbyChangeRequestMessageHandler(ICharacterActorReferenceContainer actorContainer, 
			ILobbyEntryService lobbyEntryService)
		{
			ActorContainer = actorContainer ?? throw new ArgumentNullException(nameof(actorContainer));
			LobbyEntryService = lobbyEntryService ?? throw new ArgumentNullException(nameof(lobbyEntryService));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockLobbyChangeRequestPayload message, CancellationToken token = default)
		{
			//Assume that the item id is the lobby id
			//that they want to switch to.
			int lobbyId = (int) message.Selection.ItemId;

			//Dispose of the current actor, we're gonna be leaving the lobby.
			await ActorContainer.DisposeAsync();

			//Good chance the lobby will be full.
			if (!await LobbyEntryService.TryEnterLobbyAsync(context, ActorContainer.EntityGuid, lobbyId, token))
			{
				//TODO: Better handle try join ANY lobby.
				//We don't know what lobby they were in, so let's try to put them in the first lobby and go down the list.
				for(int i = 0; i < 15; i++)
					if (await LobbyEntryService.TryEnterLobbyAsync(context, ActorContainer.EntityGuid, i, token))
						return;
			}
		}
	}
}
