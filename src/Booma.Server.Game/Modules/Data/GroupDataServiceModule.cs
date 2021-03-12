using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Glader.ASP.GameConfig;
using Glader.ASP.RPG;

namespace Booma
{
	/// <summary>
	/// Service module that registers <see cref="IGroupManagementService"/> via the <see cref="IServiceResolver{TServiceInterfaceType}"/>.
	/// </summary>
	public sealed class GroupDataServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Group data currently is on the CharacterDataService too, so we coulda used the other Module to register it
			builder.RegisterModule(new ServiceDiscoverableServiceModule<IGroupManagementService>(BoomaServiceType.CharacterDataService));
		}
	}
}
