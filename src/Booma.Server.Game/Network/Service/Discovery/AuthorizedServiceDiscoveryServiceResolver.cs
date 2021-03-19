using System;
using System.ComponentModel;
using System.Net.Http;
using Common.Logging;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// <see cref="IServiceDiscoveryService"/>-based implementation for service resolution.
	/// </summary>
	/// <typeparam name="TServiceType"></typeparam>
	public sealed class AuthorizedServiceDiscoveryServiceResolver<TServiceType> : DefaultServiceDiscoveryServiceResolver<TServiceType>
		where TServiceType : class
	{
		private IReadonlyAuthTokenRepository TokenRepository { get; }

		public AuthorizedServiceDiscoveryServiceResolver(IServiceDiscoveryService discoveryClient, 
			BoomaServiceType serviceType, 
			ILog logger,
			IReadonlyAuthTokenRepository tokenRepository)
			: base(discoveryClient, serviceType, logger)
		{
			if(!Enum.IsDefined(typeof(BoomaServiceType), serviceType)) throw new InvalidEnumArgumentException(nameof(serviceType), (int)serviceType, typeof(BoomaServiceType));

			TokenRepository = tokenRepository ?? throw new ArgumentNullException(nameof(tokenRepository));
		}

		protected override HttpMessageHandler BuildHttpClientHandler()
		{
			return new AuthenticatedHttpClientHandler(TokenRepository, new BypassHttpsValidationHandler());
		}
	}
}