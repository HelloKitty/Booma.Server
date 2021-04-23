using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.Indexed;
using Glader.ASP.NameQuery;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	/// <summary>
	/// Service module responsible for registering <see cref="INameQueryService"/>s
	/// and <see cref="IBoomaEntityNameDictionary"/>.
	/// </summary>
	public sealed class NameQueryServiceModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterNameDictionaryType<BoomaEntityNameDictionary, NetworkEntityGuid, EntityType>()
				.As<IBoomaEntityNameDictionary>()
				.SingleInstance();

			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<INameQueryService>(BoomaServiceType.CharacterDataService)
			{
				Mode = ServiceDiscoveryModuleMode.Default,
				UrlFactory = new NameQueryServiceBaseUrlFactory<EntityType>(EntityType.Group),
				GlobalScope = true,
				Key = EntityType.Group
			});
		}
	}

	public sealed class NameQueryServiceResolverAdapter : INameQueryService
	{
		private IServiceResolver<INameQueryService> ServiceResolver { get; }

		public NameQueryServiceResolverAdapter(IServiceResolver<INameQueryService> serviceResolver)
		{
			ServiceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
		}

		public async Task<EntityNameQueryResponse> QueryEntityNameAsync(ulong id, CancellationToken token = default)
		{
			ServiceResolveResult<INameQueryService> result = await ServiceResolver.Create(token);

			if (result.isAvailable)
				return await result.Instance.QueryEntityNameAsync(id, token);

			return new EntityNameQueryResponse(NameQueryResponseCode.GeneralServerError);
		}
	}
}
