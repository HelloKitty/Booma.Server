using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace Booma
{
	public sealed class BlockPlayerCreateGameRequestMessageHandler : GameMessageHandler<BlockPlayerCreateGameRequestPayload>
	{
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockPlayerCreateGameRequestPayload message, CancellationToken token = default)
		{
			await context.MessageService
				.SendMessageAsync(new BlockGameJoinEventPayload(0, 0, new GameSettings(DifficultyType.Normal, false, 0, SectionId.Viridia, false, 0, EpisodeType.EpisodeI, false), new PlayerInformationHeader[0]), token);
		}
	}
}
