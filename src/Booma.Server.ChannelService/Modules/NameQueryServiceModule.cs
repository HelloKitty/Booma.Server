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

			builder.RegisterType<BoomaEntityNameDictionary>()
				.As<IBoomaEntityNameDictionary>()
				.SingleInstance()
				.OnActivated(args =>
				{
					IIndex<EntityType, IServiceResolver<INameQueryService>> nameQueryServiceIndex = args
						.Context.Resolve<IIndex<EntityType, IServiceResolver<INameQueryService>>>();

					//This registers all possibly entity types to the queryable entity dictionary
					Enum.GetValues(typeof(EntityType))
						.Cast<EntityType>()
						.Select(type => new {type, resolver = GetQueryServiceOrDefault(nameQueryServiceIndex, type)})
						.Where(r => r.resolver != null)
						.ToList()
						.ForEach(resolverPair =>
						{
							args.Instance.AddService(resolverPair.type, new NameQueryServiceResolverAdapter(resolverPair.resolver));
						});
				});

			builder.RegisterModule(new ServiceDiscoverableServiceModule<INameQueryService>(BoomaServiceType.CharacterDataService)
			{
				Mode = ServiceDiscoveryModuleMode.Default,
				UrlFactory = new NameQueryServiceBaseUrlFactory(EntityType.Group),
				GlobalScope = true,
				Key = EntityType.Group
			});
		}

		private static IServiceResolver<INameQueryService> GetQueryServiceOrDefault(IIndex<EntityType, IServiceResolver<INameQueryService>> nameQueryServiceIndex, EntityType type)
		{
			nameQueryServiceIndex.TryGetValue(type, out var value);
			return value;
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
