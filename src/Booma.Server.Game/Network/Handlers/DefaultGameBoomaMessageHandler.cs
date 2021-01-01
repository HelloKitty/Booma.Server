using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Common.Logging;
using GladNet;

namespace Booma
{
	//We didn't need to implement this, but can be useful for debugging sometimes.
	/// <summary>
	/// Game implementation of <see cref="DefaultBoomaMessageHandler{TMessageReadType,TMessageWriteType,TOperationCodeType}"/>
	/// </summary>
	public sealed class DefaultGameBoomaMessageHandler : DefaultBoomaMessageHandler<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer, GameNetworkOperationCode>
	{
		public DefaultGameBoomaMessageHandler(ILog logger) 
			: base(logger)
		{

		}

		public override Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, PSOBBGamePacketPayloadClient message, CancellationToken token = new CancellationToken())
		{
			return base.HandleMessageAsync(context, message, token);
		}
	}
}
