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

		public BoomaPatchServerApplication(NetworkAddressInfo serverAddress, ILog logger)
			: base(serverAddress, logger)
		{
			Container = BuildServiceContainer(logger);
		}

		private static IContainer BuildServiceContainer([NotNull] ILog logger)
		{
			if (logger == null) throw new ArgumentNullException(nameof(logger));

			ContainerBuilder serviceBuilder = new ContainerBuilder();

			serviceBuilder
				.RegisterInstance(logger)
				.As<ILog>()
				.SingleInstance()
				.ExternallyOwned();

			//All the patch serializer stuff is stateless and can be shared
			//so we're going to SingleInstance it.
			serviceBuilder
				.RegisterType<PatchPacketSerializer>()
				.AsImplementedInterfaces()
				.SingleInstance();

			serviceBuilder.RegisterType<PatchPacketHeaderFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			serviceBuilder.RegisterType<PatchPacketHeaderSerializer>()
				.AsImplementedInterfaces()
				.SingleInstance();

			//Handlers AND handler service should be stateless so we only need single instance.
			serviceBuilder
				.RegisterType<DefaultMessageHandlerService<PSOBBPatchPacketPayloadClient, SessionMessageContext<PSOBBPatchPacketPayloadServer>>>()
				.As<IMessageHandlerService<PSOBBPatchPacketPayloadClient, SessionMessageContext<PSOBBPatchPacketPayloadServer>>>()
				.OnActivated(args =>
				{
					//TODO: Autodiscovery handlers via reflection.
					//Bind one of the default handlers
					args.Instance.Bind<PSOBBPatchPacketPayloadClient>(new DefaultPatchMessageHandler(args.Context.Resolve<ILog>()));
				})
				.SingleInstance();

			//This is just message serialization stuff. We don't need to make this instance per, it should be stateless.
			serviceBuilder
				.RegisterType<SessionMessageBuildingServiceContext<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>>()
				.AsSelf()
				.SingleInstance();

			serviceBuilder.RegisterType<InPlaceNetworkMessageDispatchingStrategy<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();

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

			//We don't actually dispose the lifetime scope right away, we attach it to the session.
			ILifetimeScope scope = Container.BeginLifetimeScope(c =>
			{
				c.RegisterInstance(context.Connection)
					.AsSelf()
					.ExternallyOwned();

				c.RegisterInstance(context.Details)
					.AsSelf()
					.ExternallyOwned();
			});

			try
			{
				//We build the session then attach the container's lifetime scope it was built from to the session right away
				//so it's disposed when the session is disposed.
				BoomaPatchManagedSession session = scope.Resolve<BoomaPatchManagedSession>();
				session.AttachDisposableResource(scope);
				return session;
			}
			catch (Exception)
			{
				//Important to dispose of container scope if we fail to build.
				scope.Dispose();
				throw;
			}
		}
	}
}