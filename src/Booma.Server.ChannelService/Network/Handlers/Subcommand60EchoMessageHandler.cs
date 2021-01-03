using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace Booma
{
	public sealed class Subcommand60EchoMessageHandler : GameRequestMessageHandler<BlockNetworkCommand60EventClientPayload, BlockNetworkCommand60EventServerPayload>
	{
		protected override async Task<BlockNetworkCommand60EventServerPayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockNetworkCommand60EventClientPayload message, CancellationToken token = default)
		{
			return new BlockNetworkCommand60EventServerPayload(message.Command);
		}
	}
}
