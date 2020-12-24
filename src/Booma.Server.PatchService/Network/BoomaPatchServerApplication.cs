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
	/// GladNet <see cref="TcpGladNetServerApplication{TManagedSessionType}"/> implementation for PSO Booma Patch Service.
	/// </summary>
	public sealed class BoomaPatchServerApplication : TcpGladNetServerApplication<BoomaPatchManagedSession>
	{
		/// <summary>
		/// Service container for the application.
		/// </summary>
		private IContainer Container { get; }

		/// <summary>
		/// The factory that can generate <see cref="BoomaPatchManagedSession"/>.
		/// </summary>
		private IPatchManagedSessionFactory SessionFactory { get; }

		public BoomaPatchServerApplication(NetworkAddressInfo serverAddress, ILog logger)
			: base(serverAddress, logger)
		{
			Container = BuildServiceContainer(logger);

			//Kinda hacky we manually resolve this but other ways suck too.
			SessionFactory = Container.Resolve<IPatchManagedSessionFactory>();
		}

		private static IContainer BuildServiceContainer([NotNull] ILog logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			ContainerBuilder serviceBuilder = new ContainerBuilder();

			//Patch service modules
			serviceBuilder.RegisterModule<DefaultLoggingServiceModule>()
				.RegisterModule<PatchSerializationServiceModule>()
				.RegisterModule<PatchMessageHandlerServiceModule>()
				.RegisterModule<PatchMessageServicesServiceModule>();

			//Obviously we need a FRESH session per lifetime.
			serviceBuilder
				.RegisterType<BoomaPatchManagedSession>()
				.InstancePerLifetimeScope();

			serviceBuilder
				.RegisterInstance(new NetworkConnectionOptions(2, 2, 1024))
				.AsSelf()
				.SingleInstance();

			return serviceBuilder.Build();
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}

		/// <inheritdoc />
		public override BoomaPatchManagedSession Create(SessionCreationContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			//Originally there was a lot of code piled into here but the factory
			//now takes care of the complex resolving of dependencies and setup for a session.
			return SessionFactory.Create(context);
		}
	}
}