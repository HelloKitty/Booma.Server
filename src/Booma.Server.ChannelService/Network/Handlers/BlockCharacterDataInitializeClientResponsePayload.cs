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
			//Sub60ClientBurstBeginEventCommand command = new Sub60ClientBurstBeginEventCommand(new byte[518]);
			//await context.MessageService.SendMessageAsync(new BlockNetworkCommand60EventServerPayload(command), token);
			//await context.MessageService.SendMessageAsync(new SetLobbyArrowsEventPayload(new LobbyArrowData[1] {new LobbyArrowData(1, 0)}), token);
		}
	}
}
