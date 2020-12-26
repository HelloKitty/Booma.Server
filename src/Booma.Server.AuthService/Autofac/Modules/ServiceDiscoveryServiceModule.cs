using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	/// <summary>
	/// Service module that registers: <see cref="IServiceDiscoveryService"/>
	/// </summary>
	public sealed class ServiceDiscoveryServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.Register(context =>
			{
				return Refit.RestService.For<IServiceDiscoveryService>(BoomaEndpointConstants.BOOMA_SERVICE_DISCOVERY_ENDPOINT);
			})
				.As<IServiceDiscoveryService>()
				.SingleInstance();
		}
	}
}
