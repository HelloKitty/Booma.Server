using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Booma.Server.CharacterDataService;
using Fasterflect;
using Glader.ASP.RPG;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;

namespace Booma.Tools.ItemPMT.JSON.DatabasePopulator
{
	class Program
	{
		static async Task Main(string[] args)
		{
			ServiceCollection services = new ServiceCollection();
			var mvcBuilder = services.AddMvc();
			Startup.RegisterGladerRPGSystem(services, mvcBuilder);
			var provider = services.BuildServiceProvider();

			using(var context = provider.GetService<DbContext>())
			{
				var itemTemplateSet = context.Set<DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>>();
				var itemSubclassSet = context.Set<DBRPGSItemSubClass<ItemClassType>>();

				//TODO: Remove absolute paths
				//Load weapon data
				var weaponData = JsonConvert.DeserializeObject<ItemPMTWeapons>(File.ReadAllText($@"C:\Users\Glader\Documents\Github\Booma.Server\Data\JSON\ItemPMT.Weapons.json"));

				//TODO: We're skipping SubClass 0 even though PSO does have some data in this group (for weapons its nonsense)
				for (int groupId = 1; groupId < weaponData.weapons.group.Count; groupId++)
				{
					for (int i = 0; i < weaponData.weapons.group[groupId].list.Count; i++)
					{
						var entry = weaponData.weapons.group[groupId].list[i];
						int key = (groupId << (1 * sizeof(byte) * 8)) + i; //Key: Group:G Index:I 0xGGII

						//If it's already inserted don't reinsert
						if ((await itemTemplateSet.FindAsync(key)) != null)
							continue;

						//First we must consider also that the subclass may not exist.
						if (await itemSubclassSet.FindAsync(ItemClassType.Weapon, groupId) == null)
						{
							var firstEntryInGroup = weaponData.weapons.group[groupId].list.FirstOrDefault();
							itemSubclassSet.Add(new DBRPGSItemSubClass<ItemClassType>(groupId, ItemClassType.Weapon, $"TODO: {(firstEntryInGroup == null ? "Unk" : firstEntryInGroup.name)}", "TODO"));
						}

						var dbEntry = new DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>(ItemClassType.Weapon, groupId, entry.name, entry.desc, PsobbQuality.Common);
						dbEntry.TrySetPropertyValue(nameof(DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>.Id), key);
						itemTemplateSet.Add(dbEntry);
						Console.WriteLine($"Inserting: {key} - {entry.name}");
					}
				}

				await context.SaveChangesAsync(true);
			}
		}
	}
}
