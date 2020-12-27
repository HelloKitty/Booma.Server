using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Autofac;
using Booma.Proxy;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="BaseBoomaServerApplication{TManagedSessionType}"/> implementation for PSO Booma Game Service.
	/// </summary>
	public sealed class BoomaGameServerApplication : BaseBoomaServerApplication<BoomaGameManagedSession>
	{
		public BoomaGameServerApplication(NetworkAddressInfo serverAddress, ILog logger) 
			: base(serverAddress, logger)
		{

		}

		/// <inheritdoc />
		protected override ContainerBuilder RegisterServices(ContainerBuilder builder)
		{
			//These are the default Game Service Modules
			builder.RegisterModule<DefaultLoggingServiceModule>()
				.RegisterModule<GameSerializationServiceModule>()
				.RegisterModule<GameMessageHandlerServiceModule>()
				.RegisterModule<ServerSessionServiceModule<BoomaGameManagedSession>>()
				.RegisterModule<GameGeneralServiceModule>()
				.RegisterModule<ServerMessagingServicesModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>>()
				.RegisterModule<NetworkCryptoServiceModule>() //This is the Blowfish services/dependencies required for network cryptography.
				.RegisterModule<ServiceDiscoveryServiceModule>();

			builder.RegisterInstance(BuildNetworkOptions())
				.AsSelf()
				.SingleInstance();

			//These are the custom service specified modules
			//TODO: Default for game service shouldn't be InPlace but stuff like Auth and Character session can use inplace.
			builder.RegisterModule<InPlaceMessageDispatchingServiceModule<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>>();
			builder.RegisterModule<LoginServiceModule<RedirectCharacterServiceOnLoginResponseEventListener>>();

			return builder;
		}

		private static NetworkConnectionOptions BuildNetworkOptions()
		{
			//This is dumb, but 8 bytes is the cipher block size so we require minimum header of 8 bytes.
			//even though we'll just temp store the buffer's 6 other bytes.
			return new NetworkConnectionOptions(2, 2, 72000);
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}
	}
}