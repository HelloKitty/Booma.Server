using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Handler for <see cref="CharacterChecksumRequestPayload"/> that sends <see cref="CharacterChecksumResponsePayload"/> as a response.
	/// </summary>
	public sealed class CharacterChecksumRequestMessageHandler : GameRequestMessageHandler<CharacterChecksumRequestPayload, CharacterChecksumResponsePayload>
	{
		protected override async Task<CharacterChecksumResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterChecksumRequestPayload message, CancellationToken token = default)
		{
			//TODO: Actually validate this checksum.
			return new CharacterChecksumResponsePayload(LoginChecksumResult.Success);
		}
	}
}
