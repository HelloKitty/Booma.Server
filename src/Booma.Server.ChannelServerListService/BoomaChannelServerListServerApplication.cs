using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Autofac;
using Booma;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="BoomaGameServerApplication"/> implementation for PSO Booma Game Server List Service.
	/// </summary>
	public sealed class BoomaChannelServerListServerApplication : BoomaGameServerApplication
	{
		/// <inheritdoc />
		public BoomaChannelServerListServerApplication(NetworkAddressInfo serverAddress, ILog logger) 
			: base(serverAddress, logger)
		{

		}

		/// <inheritdoc />
		protected override ContainerBuilder RegisterServices(ContainerBuilder builder)
		{
			base.RegisterServices(builder);

			//These are the custom service specified modules
			//TODO: Default for game service shouldn't be InPlace but stuff like Auth and Character session can use inplace.
			builder.RegisterModule<InPlaceMessageDispatchingServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>>();

			//Login module that sends ship list on login/auth.
			builder.RegisterModule<LoginServiceModule<ChannelListWelcomeEventListener>>();

			//GameServerList service handlers
			builder.RegisterModule(new GameAssemblyMessageHandlerServiceModule(GetType().Assembly));

			//TODO: This is a hacky way to get the menu selection handler.
			builder.RegisterModule(new GameAssemblyMessageHandlerServiceModule(typeof(GameServerListNetworkedMenu).Assembly));
			builder.RegisterModule<GameServerDataServiceModule>();

			return builder;
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}
	}
}