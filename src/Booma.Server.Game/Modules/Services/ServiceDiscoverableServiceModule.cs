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

		public ServiceDiscoverableServiceModule(BoomaServiceType serviceType)
		{
			if (!Enum.IsDefined(typeof(BoomaServiceType), serviceType)) throw new InvalidEnumArgumentException(nameof(serviceType), (int) serviceType, typeof(BoomaServiceType));
			ServiceType = serviceType;
		}

		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.Register<ServiceDiscoveryServiceResolver<TServiceType>>(context =>
				{
					return new ServiceDiscoveryServiceResolver<TServiceType>(context.Resolve<IServiceDiscoveryService>(), ServiceType, context.Resolve<ILog>(), context.Resolve<IReadonlyAuthTokenRepository>());
				})
				.As<IServiceResolver<TServiceType>>()
				.SingleInstance();
		}
	}
}
