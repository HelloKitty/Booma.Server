using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Base <see cref="GladNetServerApplication{TManagedSessionType,TSessionCreationContextType}"/> for the Booma backend.
	/// All Booma/PSO servers/services use TCP so this simplified type can be derived from to create a working Booma server application.
	/// </summary>
	public abstract class BaseBoomaServerApplication<TManagedSessionType> : TcpGladNetServerApplication<TManagedSessionType>
		where TManagedSessionType : ManagedSession
	{
		/// <summary>
		/// Service container for the application.
		/// </summary>
		private IContainer Container { get; }

		/// <summary>
		/// The factory that can generate sessions.
		/// </summary>
		private IManagedSessionFactory<TManagedSessionType> SessionFactory { get; }

		protected BaseBoomaServerApplication(NetworkAddressInfo serverAddress, ILog logger)
			: base(serverAddress, logger)
		{
			ContainerBuilder builder = new ContainerBuilder();
			RegisterInternalServices(builder);

			//I know this is bad, but the API ends up being good.
			// ReSharper disable once VirtualMemberCallInConstructor
			RegisterServices(builder);


			Container = builder.Build();

			//Kinda hacky we manually resolve this but other ways suck too.
			SessionFactory = Container.Resolve<IManagedSessionFactory<TManagedSessionType>>();
		}

		private ContainerBuilder RegisterInternalServices(ContainerBuilder builder)
		{
			return builder;
		}

		/// <summary>
		/// Implementer should register required services in the <see cref="ContainerBuilder"/>.
		/// Implementer should not assume the application is fully initialized at this point.
		/// Avoid access to any object members.
		/// </summary>
		/// <param name="builder">The container builder.</param>
		/// <returns>The builder.</returns>
		protected abstract ContainerBuilder RegisterServices(ContainerBuilder builder);

		/// <inheritdoc />
		public sealed override TManagedSessionType Create(SessionCreationContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return SessionFactory.Create(context);
		}
	}
}
