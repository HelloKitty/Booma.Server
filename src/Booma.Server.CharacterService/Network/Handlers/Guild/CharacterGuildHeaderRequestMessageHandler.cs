using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Force.Crc32;
using FreecraftCore.Serializer;
using GladNet;

namespace Booma
{
	//TODO: Refactor this GARBAGE code.
	/// <summary>
	/// Command message handler for <see cref="CharacterGuildHeaderRequestPayload"/>
	/// </summary>
	public sealed class CharacterGuildHeaderRequestMessageHandler : GameCommandMessageHandler<CharacterGuildHeaderRequestPayload>
	{
		/// <summary>
		/// Represents the expected size of the guild card data.
		/// </summary>
		private static uint GuildCardDataLength { get; }

		private static uint Checksum { get; }

		static CharacterGuildHeaderRequestMessageHandler()
		{
			//PSOBB sends a statically sized guild card data chunk.
			//Therefore we can serialize an empty one and determine the size.
			Span<byte> buffer = new Span<byte>(new byte[250000]);
			int offset = 0;
			GuildCardData_Serializer.Instance.Write(GuildCardData.Empty, buffer, ref offset);
			GuildCardDataLength = (uint) offset;

			//CRC32 for checksum.
			Checksum = Crc32Algorithm.Compute(buffer.Slice(0, offset).ToArray());
		}

		protected override async Task HandleCommandAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CancellationToken token = default)
		{
			await context.MessageService.SendMessageAsync(new CharacterGuildCardDataHeaderResponsePayload(GuildCardDataLength, Checksum), token);
		}
	}
}
