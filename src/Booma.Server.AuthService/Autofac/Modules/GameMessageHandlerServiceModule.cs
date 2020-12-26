using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma.Proxy;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Game service message handler service module.
	/// Registers the services and handling for message handlers.
	/// </summary>
	public sealed class GameMessageHandlerServiceModule 
		: ServerMessageHandlerServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer, DefaultBoomaMessageHandler<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer, GameNetworkOperationCode>>
	{

	}
}
