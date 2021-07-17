using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Glader.ASP.GameConfig;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	/// <summary>
	/// Service module that registers <see cref="IItemInstanceService"/> and <see cref="ICharacterInventoryService"/>
	/// via the <see cref="IServiceResolver{TServiceInterfaceType}"/>.
	/// </summary>
	public sealed class ItemServicesModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Group data currently is on the CharacterDataService too, so we coulda used the other Module to register it
			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<IItemInstanceService>(BoomaServiceType.CharacterDataService));
			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<ICharacterInventoryService>(BoomaServiceType.CharacterDataService));
		}
	}
}
