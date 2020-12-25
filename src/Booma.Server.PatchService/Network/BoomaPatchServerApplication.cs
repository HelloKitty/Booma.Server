﻿using System;
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
	/// GladNet <see cref="BaseBoomaServerApplication{TManagedSessionType}"/> implementation for PSO Booma Patch Service.
	/// </summary>
	public sealed class BoomaPatchServerApplication : BaseBoomaServerApplication<BoomaPatchManagedSession>
	{
		public BoomaPatchServerApplication(NetworkAddressInfo serverAddress, ILog logger) 
			: base(serverAddress, logger)
		{

		}

		/// <inheritdoc />
		protected override ContainerBuilder RegisterServices(ContainerBuilder builder)
		{
			//Patch service modules
			builder.RegisterModule<DefaultLoggingServiceModule>()
				.RegisterModule<PatchSerializationServiceModule>()
				.RegisterModule<PatchMessageHandlerServiceModule>()
				.RegisterModule<PatchMessageServicesServiceModule>()
				.RegisterModule<ServerSessionServiceModule<BoomaPatchManagedSession>>();

			builder
				.RegisterInstance(new NetworkConnectionOptions(2, 2, 1024))
				.AsSelf()
				.SingleInstance();

			return builder;
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}
	}
}