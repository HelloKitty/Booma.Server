using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Booma.Server.CharacterDataService;
using Fasterflect;
using Glader.ASP.RPG;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Booma.Tools.PlayerStats.JSON.DatabasePopulator
{
	class Program
	{
		static async Task Main(string[] args)
		{
			ServiceCollection services = new ServiceCollection();
			var mvcBuilder = services.AddMvc();
			Startup.RegisterGladerRPGSystem(services, mvcBuilder);
			var provider = services.BuildServiceProvider();

			using (var context = provider.GetService<DbContext>())
			{
				var statDefaultSet = context.Set<DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, CharacterClass>>();
				var statDefaultStatsSet = context.Set<DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, CharacterClass>>();

				var data = JsonConvert.DeserializeObject<ItemPLT>(File.ReadAllText($@"C:\Users\Glader\Documents\Github\Booma.Server\Data\JSON\ItemPLT.default.json"));

				//We start at level 1
				for (int i = 0; i < data.classList.Count; i++)
				{
					CharacterClass @class = (CharacterClass)i;
					CharacterRace race = ComputeCharacterRace(@class);
					var classEntry = data.classList[i];
					
					if ((await statDefaultSet.FindAsync(1, race, @class)) == null)
					{
						var entry = new DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, CharacterClass>(1, race, @class);
						entry.SetPropertyValue(nameof(DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, CharacterClass>.Stats), new List<RPGStatValue<CharacterStatType>>());

						//Base stats
						entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.ATA, classEntry.baseStats.ata));
						entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.ATP, classEntry.baseStats.atp));
						entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.DFP, classEntry.baseStats.dfp));
						entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.EVP, classEntry.baseStats.evp));
						entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.HP, classEntry.baseStats.hp));
						entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.LCK, classEntry.baseStats.lck));
						entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.MST, classEntry.baseStats.mst));

						//Remove any 0 stats
						foreach (var statEntry in entry.Stats.Where(s => s.Value == 0).ToArray())
							entry.Stats.Remove(statEntry);

						statDefaultSet.Add(entry);
					}

					//Skip the first slot (it's empty stats)
					//For the next 200 levels we should have data
					for(int level = 2; level <= 200; level++)
					{
						if ((await statDefaultSet.FindAsync(level, race, @class)) == null)
						{
							var entry = new DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, CharacterClass>(level, race, @class);
							entry.SetPropertyValue(nameof(DBRPGCharacterStatDefault<CharacterStatType, CharacterRace, CharacterClass>.Stats), new List<RPGStatValue<CharacterStatType>>());

							//Base stats
							entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.ATA, classEntry.levels[level - 1].ata));
							entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.ATP, classEntry.levels[level - 1].atp));
							entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.DFP, classEntry.levels[level - 1].dfp));
							entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.EVP, classEntry.levels[level - 1].evp));
							entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.HP, classEntry.levels[level - 1].hp));
							entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.LCK, classEntry.levels[level - 1].lck));
							entry.Stats.Add(new RPGStatValue<CharacterStatType>(CharacterStatType.MST, classEntry.levels[level - 1].mst));

							//Remove any 0 stats
							foreach(var statEntry in entry.Stats.Where(s => s.Value == 0).ToArray())
								entry.Stats.Remove(statEntry);

							statDefaultSet.Add(entry);

							Console.WriteLine($"Adding Level: {level} Class: {@class} Race: {race}");
						}
					}
				}

				await context.SaveChangesAsync(true);
			}
		}

		//TODO: Refactor to Booma.Proxy
		private static CharacterRace ComputeCharacterRace(CharacterClass @class)
		{
			switch(@class)
			{
				case CharacterClass.RAmarl:
				case CharacterClass.FOmarl:
				case CharacterClass.RAmar:
				case CharacterClass.FOmar:
				case CharacterClass.HUmar:
					return CharacterRace.Human;
				case CharacterClass.HUcaseal:
				case CharacterClass.RAcaseal:
				case CharacterClass.RAcast:
				case CharacterClass.HUcast:
					return CharacterRace.Cast;
				case CharacterClass.HUnewearl:
				case CharacterClass.FOnewm:
				case CharacterClass.FOnewearl:
					return CharacterRace.Newman;
				default:
					throw new ArgumentOutOfRangeException(nameof(@class), @class, null);
			}
		}
	}
}
