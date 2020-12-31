using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Booma
{
	public sealed class GameServerDataServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<GlobalServiceDiscoveryServerEntryRepository>()
				.As<IGameServerEntryRepository>()
				.InstancePerLifetimeScope();

			builder.RegisterType<GameServerListNetworkedMenu>()
				.AsSelf()
				.InstancePerLifetimeScope();
		}
	}
}
