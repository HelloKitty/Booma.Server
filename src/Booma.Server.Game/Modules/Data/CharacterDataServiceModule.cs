using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Glader.ASP.GameConfig;
using Glader.ASP.RPG;

//TODO: When Refit fixes: https://github.com/reactiveui/refit/issues/931 we should use closed derived Type.
using IPSOBBCharacterCreationService = Glader.ASP.RPG.ICharacterCreationService<Booma.CharacterRace, Booma.CharacterClass>;
using IPSOBBCharacterDataQueryService = Glader.ASP.RPG.ICharacterDataQueryService<Booma.CharacterRace, Booma.CharacterClass>;

namespace Booma
{
	/// <summary>
	/// Service module that registers <see cref="ICharacterDataQueryService{TRaceType,TClassType}"/>
	/// and <see cref="ICharacterCreationService{TRaceType,TClassType}"/> via the <see cref="IServiceResolver{TServiceInterfaceType}"/>.
	/// </summary>
	public sealed class CharacterDataServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<IPSOBBCharacterDataQueryService>(BoomaServiceType.CharacterDataService));
			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<IPSOBBCharacterCreationService>(BoomaServiceType.CharacterDataService));
			builder.RegisterModule(new BoomaServiceDiscoverableServiceModule<IPSOBBCharacterAppearanceService>(BoomaServiceType.CharacterDataService));

			//TODO: If Groups ever move off the CharacterDataService we should change the location of this registeration.
			builder.RegisterModule<GroupDataServiceModule>();

			//CharacterDataServiceBackedCharacterDataRepository : ICharacterDataRepository
			builder.RegisterType<CharacterDataServiceBackedCharacterDataRepository>()
				.As<ICharacterDataRepository>()
				.InstancePerLifetimeScope();
		}
	}
}
