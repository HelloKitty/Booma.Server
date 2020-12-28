using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using Common.Logging;
using GladNet;

namespace Booma
{
	//TODO: Need persistence.
	//Client can send this if it disagrees with the options data sent down or wants to update their options.
	/// <summary>
	/// Message handler for <see cref="CharacterOptionsRequestPayload"/> which returns a <see cref="CharacterOptionsResponsePayload"/>.
	/// </summary>
	public sealed class CharacterOptionsUpdateRequestMessageHandler : GameRequestMessageHandler<CharacterOptionsUpdateRequestPayload, CharacterOptionsResponsePayload>
	{
		protected override async Task<CharacterOptionsResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterOptionsUpdateRequestPayload message, CancellationToken token = default)
		{
			//TODO: We should save and persist these options.
			//Just ehco it back as an options response for now.
			return new CharacterOptionsResponsePayload(message.Bindings, message.GuildCard, message.TeamInfo);
		}
	}
}
