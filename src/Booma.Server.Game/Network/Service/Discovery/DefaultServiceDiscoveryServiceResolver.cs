using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Glader.ASP.ServiceDiscovery;
using Nito.AsyncEx;
using Refit;

namespace Booma
{
	public class DefaultServiceDiscoveryServiceResolver<TServiceType> : IServiceResolver<TServiceType>
		where TServiceType : class
	{
		/// <summary>
		/// Client for service discovery.
		/// </summary>
		private IServiceDiscoveryService DiscoveryClient { get; }

		/// <summary>
		/// Indicates the booma service type the <typeparamref name="TServiceType"/> is mapped to.
		/// </summary>
		public BoomaServiceType ServiceType { get; }

		/// <summary>
		/// The async syncronization object.
		/// </summary>
		protected AsyncReaderWriterLock SyncObj { get; } = new AsyncReaderWriterLock();

		/// <summary>
		/// Represents an existing resolved service instance.
		/// (default is unavailable)
		/// </summary>
		protected ServiceResolveResult<TServiceType> ServiceInstance { get; private set; } = new ServiceResolveResult<TServiceType>();

		/// <summary>
		/// Logging service.
		/// </summary>
		protected ILog Logger { get; }

		public DefaultServiceDiscoveryServiceResolver(IServiceDiscoveryService discoveryClient,
			BoomaServiceType serviceType,
			ILog logger)
		{
			if(!Enum.IsDefined(typeof(BoomaServiceType), serviceType)) throw new InvalidEnumArgumentException(nameof(serviceType), (int)serviceType, typeof(BoomaServiceType));

			DiscoveryClient = discoveryClient ?? throw new ArgumentNullException(nameof(discoveryClient));
			ServiceType = serviceType;
			Logger = logger;
		}

		public async Task<ServiceResolveResult<TServiceType>> Create(CancellationToken context)
		{
			//If not available we must resolve.
			if(!ServiceInstance.isAvailable)
				await TryCreateService(context);

			//No matter what at this point we will return whatever instance is available
			using(await SyncObj.ReaderLockAsync(context))
				return ServiceInstance;
		}

		protected async Task<bool> TryCreateService(CancellationToken token)
		{
			using(await SyncObj.WriterLockAsync(token))
			{
				//Double check locking, may have been discovered in the middle of checking.
				if(ServiceInstance.isAvailable)
					return true;

				//TODO: Add cancel token to service discovery
				string name = String.Empty;
				try
				{
					name = BoomaEndpointConstants.GetServiceIdentifier(ServiceType);
					var discoveryResponse = await DiscoveryClient.DiscoverServiceAsync(name, token);

					//Failed to discover the service endpoint so service
					//is unavailable, return existing unavailable model.
					if(!discoveryResponse.isSuccessful)
						return false;

					//We have a valid endpoint!! Let's build the service
					ServiceInstance = BuildService(discoveryResponse.Result);
					return true;
				}
				catch(Exception e)
				{
					if(Logger.IsFatalEnabled)
						Logger.Fatal($"Failed to resolve Service: {typeof(TServiceType).Name} Name: {name} Reason: {e}");

					//Return existing instance.
					return false;
				}
			}
		}

		private ServiceResolveResult<TServiceType> BuildService(ResolvedEndpoint endpoint)
		{
			if(endpoint == null) throw new ArgumentNullException(nameof(endpoint));

			TServiceType service = RestService
				.For<TServiceType>($"{endpoint.Address}:{endpoint.Port}", new RefitSettings() { HttpMessageHandlerFactory = BuildHttpClientHandler });

			return new ServiceResolveResult<TServiceType>(service);
		}

		protected virtual HttpMessageHandler BuildHttpClientHandler()
		{
			return null;
		}
	}
}