using System;
using System.Collections.Generic;
using System.Linq;
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
		/// <inheritdoc />
		protected override void RegisterHandlers(ContainerBuilder builder)
		{
			base.RegisterHandlers(builder);
			builder.RegisterModule(new GameAssemblyMessageHandlerServiceModule(typeof(GameMessageHandlerServiceModule).Assembly));
		}
	}
}
