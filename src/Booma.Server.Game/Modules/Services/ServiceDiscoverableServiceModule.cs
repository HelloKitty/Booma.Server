using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Autofac;
using Common.Logging;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;

namespace Booma
{
	public enum ServiceDiscoveryModuleMode
	{
		Default = 1,
		Authorized = 2
	}

	/// <summary>
	/// Service module that registers a <see cref="IServiceResolver{TServiceInterfaceType}"/> for the specified
	/// <typeparamref name="TServiceType"/>. Using <see cref="IServiceDiscoveryService"/> to resolve them.
	/// </summary>
	/// <typeparam name="TServiceType"></typeparam>
	public sealed class ServiceDiscoverableServiceModule<TServiceType> : Module 
		where TServiceType : class
	{
		/// <summary>
		/// The Booma service type mapping.
		/// </summary>
		private BoomaServiceType ServiceType { get; }

		//Oddly "Authorized" is our default due to legacy reasons.
		public ServiceDiscoveryModuleMode Mode { get; init; } = ServiceDiscoveryModuleMode.Authorized;

		public IServiceBaseUrlFactory UrlFactory { get; init; } = new DefaultServiceBaseUrlFactory();

		public bool GlobalScope { get; init; } = false;

		public object Key { get; init; } = null;

		public ServiceDiscoverableServiceModule(BoomaServiceType serviceType)
		{
			if (!Enum.IsDefined(typeof(BoomaServiceType), serviceType)) throw new InvalidEnumArgumentException(nameof(serviceType), (int) serviceType, typeof(BoomaServiceType));
			ServiceType = serviceType;
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			if (Mode == ServiceDiscoveryModuleMode.Authorized)
			{
				//I know this seems strange, but having the service provider be unique to the
				//lifetimescope means we can inject Auth tokens into it and not worry about sharing this resource.
				//It also means if a service dies or gets removed, reconnecting will yield another service on connection.
				var reg = builder.Register<AuthorizedServiceDiscoveryServiceResolver<TServiceType>>(context =>
					{
						return new AuthorizedServiceDiscoveryServiceResolver<TServiceType>(context.Resolve<IServiceDiscoveryService>(), ServiceType, context.Resolve<ILog>(), context.Resolve<IReadonlyAuthTokenRepository>(), UrlFactory);
					})
					.As<IServiceResolver<TServiceType>>();

				if (GlobalScope)
					reg = reg.SingleInstance();
				else
					reg = reg.InstancePerLifetimeScope();
			}
			else
			{
				var reg = builder.Register<DefaultServiceDiscoveryServiceResolver<TServiceType>>(context =>
					{
						return new DefaultServiceDiscoveryServiceResolver<TServiceType>(context.Resolve<IServiceDiscoveryService>(), ServiceType, context.Resolve<ILog>(), UrlFactory);
					})
					.As<IServiceResolver<TServiceType>>();

				if (GlobalScope)
					reg = reg.SingleInstance();
				else
					reg = reg.InstancePerLifetimeScope();

				if (Key != null)
					reg.Keyed<IServiceResolver<TServiceType>>(Key);
			}
		}
	}
}
