﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using GGDBF;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using Nito.AsyncEx;
using Refit;

namespace Booma
{
	public sealed class GGDBFInitializer
	{
		private IServiceDiscoveryService ServiceDiscoveryClient { get; }

		private ILog Logger { get; }

		private AsyncLock Lock { get; } = new AsyncLock();

		public GGDBFInitializer(IServiceDiscoveryService serviceDiscoveryClient, ILog logger)
		{
			ServiceDiscoveryClient = serviceDiscoveryClient ?? throw new ArgumentNullException(nameof(serviceDiscoveryClient));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task InitializeAsync(CancellationToken token = default)
		{
			using (await Lock.LockAsync())
			{
				var endpointResponse = await ServiceDiscoveryClient.DiscoverServiceAsync(BoomaEndpointConstants.GetServiceIdentifier(BoomaServiceType.CharacterDataService), token);

				if(endpointResponse == null)
					throw new InvalidOperationException($"Failed to retrieve endpoint for ServiceType: {BoomaServiceType.CharacterDataService}");

				var dataSource = new RefitHttpGGDBFDataSource<RPGStaticDataContext<DefaultTestSkillType, CharacterRace, CharacterClass, PsobbProportionSlots, PsobbCustomizationSlots, CharacterStatType, ItemClassType, PsobbQuality, Vector3<byte>>>($"{endpointResponse.Result.Address}:{endpointResponse.Result.Port}", new RefitHttpGGDBFDataSourceOptions()
				{
					RefreshOnFirstQuery = true,
					Settings = new RefitSettings()
					{
						HttpMessageHandlerFactory = () => new BypassHttpsValidationHandler()
					}
				});

				await RPGStaticDataContext<DefaultTestSkillType, CharacterRace, CharacterClass, PsobbProportionSlots, PsobbCustomizationSlots, CharacterStatType, ItemClassType, PsobbQuality, Vector3<byte>>.Initialize(dataSource);

				if(Logger.IsInfoEnabled)
					Logger.Info($"GGDBF Initialized");
			}
		}
	}
}
