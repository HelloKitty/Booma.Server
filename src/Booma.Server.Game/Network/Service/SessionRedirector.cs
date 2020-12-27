using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Booma.Proxy;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using GladNet;

namespace Booma
{
	public interface ISessionRedirectorFactory : IFactoryCreatable<ISessionRedirector, EmptyFactoryContext>
	{

	}

	/// <summary>
	/// Builds <see cref="SessionRedirector"/>s.
	/// </summary>
	public sealed class DefaultSessionRedirectorFactory : ISessionRedirectorFactory
	{
		/// <summary>
		/// The service discovery client.
		/// </summary>
		private IServiceDiscoveryService ServiceDiscoveryClient { get; }

		public DefaultSessionRedirectorFactory([NotNull] IServiceDiscoveryService serviceDiscoveryClient)
		{
			ServiceDiscoveryClient = serviceDiscoveryClient ?? throw new ArgumentNullException(nameof(serviceDiscoveryClient));
		}

		/// <inheritdoc />
		public ISessionRedirector Create(EmptyFactoryContext context)
		{
			return new SessionRedirector(ServiceDiscoveryClient);
		}
	}

	/// <summary>
	/// Contract for types that can redirect a session.
	/// </summary>
	public interface ISessionRedirector
	{
		/// <summary>
		/// Attempts to redirect a session via their <see cref="session"/> to the <see cref="redirectionTarget"/>.
		/// </summary>
		/// <param name="redirectionTarget">The target service to redirect to.</param>
		/// <param name="session">Session to redirect.</param>
		/// <returns>True if redirection was successful.</returns>
		Task<bool> RedirectAsync(BoomaServiceType redirectionTarget, IMessageSendService<PSOBBGamePacketPayloadServer> session);
	}

	/// <summary>
	/// Provides session redirection logic.
	/// </summary>
	public sealed class SessionRedirector : ISessionRedirector
	{
		private IServiceDiscoveryService ServiceDiscoveryClient { get; }

		public SessionRedirector([NotNull] IServiceDiscoveryService serviceDiscoveryClient)
		{
			ServiceDiscoveryClient = serviceDiscoveryClient ?? throw new ArgumentNullException(nameof(serviceDiscoveryClient));
		}

		/// <inheritdoc />
		public async Task<bool> RedirectAsync(BoomaServiceType redirectionTarget, [NotNull] IMessageSendService<PSOBBGamePacketPayloadServer> session)
		{
			if (session == null) throw new ArgumentNullException(nameof(session));
			if(!Enum.IsDefined(typeof(BoomaServiceType), redirectionTarget)) throw new InvalidEnumArgumentException(nameof(redirectionTarget), (int)redirectionTarget, typeof(BoomaServiceType));

			string serviceName = BoomaEndpointConstants.GetServiceIdentifier(redirectionTarget);

			var serviceDiscoveryResult = await ServiceDiscoveryClient.DiscoverServiceAsync(serviceName);

			if (!serviceDiscoveryResult.isSuccessful)
				return false;

			await session.SendMessageAsync(new SharedConnectionRedirectPayload(BuildValidRedirectionAddress(serviceDiscoveryResult), (short)serviceDiscoveryResult.Result.Port));
			return true;
		}

		private static string BuildValidRedirectionAddress(ResponseModel<ResolvedEndpoint, ResolvedServiceEndpointResponseCode> serviceDiscoveryResult)
		{
			//TODO: Should we guard against stuff like HTTP in the address? This must translate to an IP.
			return $"{serviceDiscoveryResult.Result.Address}";
		}
	}
}
