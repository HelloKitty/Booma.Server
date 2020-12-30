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
	/// GladNet <see cref="BoomaGameServerApplication"/> implementation for PSO Booma Character Service.
	/// </summary>
	public sealed class BoomaCharacterServerApplication : BoomaGameServerApplication
	{
		/// <inheritdoc />
		public BoomaCharacterServerApplication(NetworkAddressInfo serverAddress, ILog logger) 
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

			//Login module that redirects to the ship list if they're on that stage.
			builder.RegisterModule<LoginServiceModule<RedirectShipListLoginResponseEventListener>>();

			//Character service handlers
			builder.RegisterModule(new GameAssemblyMessageHandlerServiceModule(GetType().Assembly));

			//Data content such as characters and Parameters
			builder.RegisterModule<ParameterContentServiceModule>();
			builder.RegisterModule<CharacterDataServiceModule>();

			return builder;
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}
	}
}