using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	/// <summary>
	/// Implementation of <see cref="IGameServerEntryRepository"/> based around the global
	/// <see cref="IServiceDiscoveryService"/>.
	/// </summary>
	public sealed class GlobalServiceDiscoveryServerEntryRepository : IGameServerEntryRepository
	{
		/// <summary>
		/// Discovery service.
		/// </summary>
		private IServiceDiscoveryService ServiceDiscoveryClient { get; }

		private ILog Logger { get; }

		public GlobalServiceDiscoveryServerEntryRepository(IServiceDiscoveryService serviceDiscoveryClient, 
			ILog logger)
		{
			ServiceDiscoveryClient = serviceDiscoveryClient ?? throw new ArgumentNullException(nameof(serviceDiscoveryClient));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public async Task<ConnectionEntry[]> RetrieveAllAsync(CancellationToken token = default)
		{
			string shipServiceType = BoomaEndpointConstants.GetServiceIdentifier(BoomaServiceType.ShipService);
			var discoveryResponse = await ServiceDiscoveryClient.DiscoverServicesAsync(shipServiceType, token);

			if (!discoveryResponse.isSuccessful)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Failed to resolve ship list. Reason: {discoveryResponse.ResultCode}");

				return Array.Empty<ConnectionEntry>();
			}

			return discoveryResponse
				.Result
				.Services
				.Select(s => new ConnectionEntry(s.Name, s.Endpoint))
				.ToArray();
		}
	}
}
