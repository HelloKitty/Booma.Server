using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Absolute ridiculous data snapshop of PSOBB character data.
	/// </summary>
	public sealed class InitialCharacterDataSnapshot
	{
		public CharacterInventoryData Inventory { get; }

		public CharacterBankData Bank { get; }

		public CharacterStats Stats { get; }

		public CharacterProgress Progress { get; }

		public CharacterSpecialCustomInfo SpecialCustom { get; }

		public CharacterVersionData Version { get; }

		public CharacterCustomizationInfo Customization { get; }

		public GuildCardEntry GuildCard { get; }

		public CharacterOptionsConfiguration Options { get; }

		public string Name => GuildCard.Name;

		public NetworkEntityGuid EntityGuid { get; }

		public InitialCharacterDataSnapshot(CharacterInventoryData inventory, CharacterBankData bank, CharacterStats stats, CharacterProgress progress, CharacterSpecialCustomInfo specialCustom, CharacterVersionData version, CharacterCustomizationInfo customization, GuildCardEntry guildCard, CharacterOptionsConfiguration options, NetworkEntityGuid entityGuid)
		{
			Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory));
			Bank = bank ?? throw new ArgumentNullException(nameof(bank));
			Stats = stats ?? throw new ArgumentNullException(nameof(stats));
			Progress = progress ?? throw new ArgumentNullException(nameof(progress));
			SpecialCustom = specialCustom ?? throw new ArgumentNullException(nameof(specialCustom));
			Version = version ?? throw new ArgumentNullException(nameof(version));
			Customization = customization ?? throw new ArgumentNullException(nameof(customization));
			GuildCard = guildCard ?? throw new ArgumentNullException(nameof(guildCard));
			Options = options ?? throw new ArgumentNullException(nameof(options));
			EntityGuid = entityGuid ?? throw new ArgumentNullException(nameof(entityGuid));
		}
	}
}
