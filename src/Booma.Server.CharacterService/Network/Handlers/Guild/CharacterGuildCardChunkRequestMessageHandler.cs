using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using FreecraftCore.Serializer;
using GladNet;

namespace Booma
{
	//TODO: Refactor this GARBAGE code.
	public sealed class CharacterGuildCardChunkRequestMessageHandler : GameRequestMessageHandler<CharacterGuildCardChunkRequestPayload, CharacterGuildCardChunkResponsePayload>
	{
		protected override async Task<CharacterGuildCardChunkResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterGuildCardChunkRequestPayload message, CancellationToken token = default)
		{
			if (!message.ShouldContinue)
				return null;

			//From Sylverant chunk sending.
			//uint32_t offset = (chunk * 0x6800);
			byte[] chunk = ComputeChunk((int)message.ChunkNumber);

			return new CharacterGuildCardChunkResponsePayload(message.ChunkNumber, chunk);
		}

		//TODO: Optimize performance.
		private static byte[] ComputeChunk(int messageChunkNumber)
		{
			//PSOBB sends a statically sized guild card data chunk.
			//Therefore we can serialize an empty one and determine the size.
			Span<byte> buffer = new Span<byte>(new byte[250000]);
			int offset = 0;
			GuildCardData_Serializer.Instance.Write(GuildCardData.Empty, buffer, ref offset);
			buffer = buffer.Slice(0, offset);

			int start = messageChunkNumber * 0x6800;
			return buffer
				.Slice(start, Math.Min(0x6800, buffer.Length - messageChunkNumber * 0x6800))
				.ToArray();
		}
	}
}
