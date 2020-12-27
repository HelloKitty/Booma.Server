using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Autofac;
using Glader.ASP.ServiceDiscovery;
using Refit;

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

			//https://stackoverflow.com/questions/4926676/mono-https-webrequest-fails-with-the-authentication-or-decryption-has-failed
			ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
			ServicePointManager.CheckCertificateRevocationList = false;

			builder.Register(context =>
			{
				return RestService.For<IServiceDiscoveryService>(BoomaEndpointConstants.BOOMA_SERVICE_DISCOVERY_ENDPOINT, new RefitSettings() { HttpMessageHandlerFactory = () => new BypassHttpsValidationHandler() });
			})
				.As<IServiceDiscoveryService>()
				.SingleInstance();
		}
	}
}
