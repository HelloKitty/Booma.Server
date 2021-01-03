using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace Booma.Server
{
	/// <summary>
	/// Client sends <see cref="BlockCharacterDataInitializeClientResponsePayload"/> after <see cref="InitializeCharacterDataEventPayload"/>.
	/// We should validate and then disconnect or we should send the lobby list.
	/// </summary>
	public sealed class BlockCharacterDataInitializeClientResponseMessageHandler : GameMessageHandler<BlockCharacterDataInitializeClientResponsePayload>
	{
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockCharacterDataInitializeClientResponsePayload message, CancellationToken token = new CancellationToken())
		{
			await context.MessageService.SendMessageAsync(new BlockLobbyJoinEventPayload(0, 0, 0, 1, 0, new CharacterJoinData[1]
			{
				new CharacterJoinData(new PlayerInformationHeader(1, 0, "Glader"), new CharacterInventoryData(0, 0, 0, 0, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), ChannelWelcomeEventListener.CreateDefaultCharacterData()),
			}), token);

			//Sub60ClientBurstBeginEventCommand command = new Sub60ClientBurstBeginEventCommand(new byte[518]);
			//await context.MessageService.SendMessageAsync(new BlockNetworkCommand60EventServerPayload(command), token);
			//await context.MessageService.SendMessageAsync(new SetLobbyArrowsEventPayload(new LobbyArrowData[1] {new LobbyArrowData(1, 0)}), token);
		}
	}
}
