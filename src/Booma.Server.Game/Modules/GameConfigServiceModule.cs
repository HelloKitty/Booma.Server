using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Glader.ASP.GameConfig;

namespace Booma
{
	public sealed class GameConfigServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//This has config like keybinds and stuff, which character service has to deal with.
			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<IGameConfigurationService<PsobbGameConfigurationType>>(BoomaServiceType.GameConfigurationService));

			//Depends on service resolver so must be instance per lifetimescope.
			//DefaultCharacterOptionsConfigurationFactory : ICharacterOptionsConfigurationFactory
			builder.RegisterType<DefaultCharacterOptionsConfigurationFactory>()
				.As<ICharacterOptionsConfigurationFactory>()
				.InstancePerLifetimeScope();
		}
	}
}
