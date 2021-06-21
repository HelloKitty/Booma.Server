using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GGDBF;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;

namespace Booma
{
	public interface IBoomaGGDBFData : IRPGStaticDataContext<DefaultTestSkillType, CharacterRace, CharacterClass, PsobbProportionSlots, PsobbCustomizationSlots, CharacterStatType>
	{

	}

	public sealed class BoomaGGDBFAdapter : IBoomaGGDBFData
	{
		private IRPGStaticDataContext<DefaultTestSkillType, CharacterRace, CharacterClass, PsobbProportionSlots, PsobbCustomizationSlots, CharacterStatType> AdaptedContext { get; }

		public IReadOnlyDictionary<DefaultTestSkillType, DBRPGSkill<DefaultTestSkillType>> Skill => AdaptedContext.Skill;

		public IReadOnlyDictionary<CharacterRace, DBRPGRace<CharacterRace>> Race => AdaptedContext.Race;

		public IReadOnlyDictionary<CharacterClass, DBRPGClass<CharacterClass>> Class => AdaptedContext.Class;

		public IReadOnlyDictionary<PsobbCustomizationSlots, DBRPGCharacterCustomizableSlotType<PsobbCustomizationSlots>> CharacterCustomizationSlotType => AdaptedContext.CharacterCustomizationSlotType;

		public IReadOnlyDictionary<PsobbProportionSlots, DBRPGCharacterProportionSlotType<PsobbProportionSlots>> CharacterProportionSlotType => AdaptedContext.CharacterProportionSlotType;

		public IReadOnlyDictionary<CharacterStatType, DBRPGStat<CharacterStatType>> Stat => AdaptedContext.Stat;

		public IReadOnlyDictionary<DBRPGCharacterStatDefaultKey<CharacterStatType, CharacterRace, CharacterClass>, DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, CharacterClass>> CharacterStatDefault => AdaptedContext.CharacterStatDefault;
	}

	public sealed class GGDBFDataModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<BoomaGGDBFAdapter>()
				.As<IBoomaGGDBFData>()
				.SingleInstance();

			builder.RegisterType<InitializeGGDBFInitializable>()
				.As<IGameInitializable>()
				.SingleInstance();
		}
	}
}
