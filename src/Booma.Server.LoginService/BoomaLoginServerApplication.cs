﻿using System;
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
	/// GladNet <see cref="BaseBoomaServerApplication{TManagedSessionType}"/> implementation for PSO Booma Game Service.
	/// </summary>
	public sealed class BoomaLoginServerApplication : BoomaGameServerApplication
	{
		public BoomaLoginServerApplication(NetworkAddressInfo serverAddress, ILog logger) 
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
			builder.RegisterModule<LoginServiceModule<RedirectCharacterServiceOnLoginResponseEventListener>>();

			return builder;
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}
	}
}