﻿using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Glader.ASP.GameConfig;
using Glader.ASP.RPGCharacter;

namespace Booma
{
	/// <summary>
	/// Service module that registers <see cref="ICharacterDataQueryService"/>
	/// and <see cref="ICharacterCreationService"/> via the <see cref="IServiceResolver{TServiceInterfaceType}"/>.
	/// </summary>
	public sealed class CharacterDataServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule(new ServiceDiscoverableServiceModule<ICharacterDataQueryService>(BoomaServiceType.CharacterDataService));
			builder.RegisterModule(new ServiceDiscoverableServiceModule<ICharacterCreationService>(BoomaServiceType.CharacterDataService));

			//This has config like keybinds and stuff, which character service has to deal with.
			builder.RegisterModule(new ServiceDiscoverableServiceModule<IKeybindConfigurationService>(BoomaServiceType.GameConfigurationService));

			//CharacterDataServiceBackedCharacterDataRepository : ICharacterDataRepository
			builder.RegisterType<CharacterDataServiceBackedCharacterDataRepository>()
				.As<ICharacterDataRepository>()
				.InstancePerLifetimeScope();
		}
	}
}
