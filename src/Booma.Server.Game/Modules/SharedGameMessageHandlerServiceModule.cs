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
	/// Default Game service message handler service module.
	/// Registers the services and handling for message handlers.
	/// Registers all shared/default handlers.
	/// </summary>
	public sealed class SharedGameMessageHandlerServiceModule 
		: ServerMessageHandlerServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer, DefaultGameBoomaMessageHandler>
	{
		/// <inheritdoc />
		protected override void RegisterHandlers(ContainerBuilder builder)
		{
			base.RegisterHandlers(builder);
			builder.RegisterModule(new GameAssemblyMessageHandlerServiceModule(typeof(SharedGameMessageHandlerServiceModule).Assembly));
		}
	}
}
