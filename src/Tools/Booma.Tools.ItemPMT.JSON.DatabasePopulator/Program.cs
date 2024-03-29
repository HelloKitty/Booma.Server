﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Booma.Server.CharacterDataService;
using Booma.Tools.ItemPMT.JSON.Barriers;
using Booma.Tools.ItemPMT.JSON.Frames;
using Booma.Tools.ItemPMT.JSON.Tools;
using Booma.Tools.ItemPMT.JSON.Units;
using Booma.Tools.ItemPMT.JSON.Weapons;
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

				await LoadWeaponEntriesAsync(itemTemplateSet, itemSubclassSet);
				await LoadFrameEntriesAsync(itemTemplateSet, itemSubclassSet);
				await LoadBarrierEntriesAsync(itemTemplateSet, itemSubclassSet);
				await LoadUnitsEntriesAsync(itemTemplateSet, itemSubclassSet);
				await LoadToolEntriesAsync(itemTemplateSet, itemSubclassSet);

				await context.SaveChangesAsync(true);
			}
		}

		private static async Task LoadToolEntriesAsync(DbSet<DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>> itemTemplateSet, DbSet<DBRPGSItemSubClass<ItemClassType>> itemSubclassSet)
		{
			//TODO: Remove absolute paths
			var toolData = JsonConvert.DeserializeObject<ItemPMTTools>(File.ReadAllText($@"C:\Users\Glader\Documents\Github\Booma.Server\Data\JSON\ItemPMT.Tools.json"));

			for(int groupId = 0; groupId < toolData.tools.@group.Count; groupId++)
			{
				for(int i = 0; i < toolData.tools.@group[groupId].list.Count; i++)
				{
					var entry = toolData.tools.@group[groupId].list[i];
					var key = CreateItemTemplateKey(ItemClassType.Tool, groupId, i);

					//If it's already inserted don't reinsert
					if((await itemTemplateSet.FindAsync(key)) != null)
						continue;

					await CreateSubclassIfNeededAsync(ItemClassType.Tool, itemSubclassSet, groupId);

					//TODO: HACK because RPG lib cannot support subclasses less than 1 currently (design will change)
					var dbEntry = new DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>(ItemClassType.Tool, Math.Max(1, groupId), entry.name, entry.desc, PsobbQuality.Common);

					if (groupId == 0)
						dbEntry.SetPropertyValue(nameof(DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>.SubClassId), groupId);

					dbEntry.TrySetPropertyValue(nameof(DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>.Id), key);
					itemTemplateSet.Add(dbEntry);
					Console.WriteLine($"Inserting: {key} - {entry.name}");
				}
			}
		}

		private static async Task LoadUnitsEntriesAsync(DbSet<DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>> itemTemplateSet, DbSet<DBRPGSItemSubClass<ItemClassType>> itemSubclassSet)
		{
			var unitData = JsonConvert.DeserializeObject<ItemPMTUnits>(File.ReadAllText($@"C:\Users\Glader\Documents\Github\Booma.Server\Data\JSON\ItemPMT.Units.json"));

			for(int i = 0; i < unitData.units.list.Count; i++)
			{
				var entry = unitData.units.list[i];
				var key = CreateItemTemplateKey(ItemClassType.Guard, 3, i); //Subclass 3 is Unit

				//If it's already inserted don't reinsert
				if((await itemTemplateSet.FindAsync(key)) != null)
					continue;

				await CreateSubclassIfNeededAsync(ItemClassType.Guard, itemSubclassSet, 3, "Unit");

				var dbEntry = new DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>(ItemClassType.Guard, 3, entry.name, entry.desc, PsobbQuality.Common);
				dbEntry.TrySetPropertyValue(nameof(DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>.Id), key);
				itemTemplateSet.Add(dbEntry);
				Console.WriteLine($"Inserting: {key} - {entry.name}");
			}
		}

		private static async Task LoadBarrierEntriesAsync(DbSet<DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>> itemTemplateSet, DbSet<DBRPGSItemSubClass<ItemClassType>> itemSubclassSet)
		{
			var barrierData = JsonConvert.DeserializeObject<ItemPMTBarriers>(File.ReadAllText($@"C:\Users\Glader\Documents\Github\Booma.Server\Data\JSON\ItemPMT.Barriers.json"));

			for(int i = 0; i < barrierData.barriers.list.Count; i++)
			{
				var entry = barrierData.barriers.list[i];
				var key = CreateItemTemplateKey(ItemClassType.Guard, 2, i); //Subclass 2 is Barrier

				//If it's already inserted don't reinsert
				if((await itemTemplateSet.FindAsync(key)) != null)
					continue;

				await CreateSubclassIfNeededAsync(ItemClassType.Guard, itemSubclassSet, 2, "Barrier");

				var dbEntry = new DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>(ItemClassType.Guard, 2, entry.name, entry.desc, PsobbQuality.Common);
				dbEntry.TrySetPropertyValue(nameof(DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>.Id), key);
				itemTemplateSet.Add(dbEntry);
				Console.WriteLine($"Inserting: {key} - {entry.name}");
			}
		}

		private static async Task LoadFrameEntriesAsync(DbSet<DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>> itemTemplateSet, DbSet<DBRPGSItemSubClass<ItemClassType>> itemSubclassSet)
		{
			var frameData = JsonConvert.DeserializeObject<ItemPMTFrames>(File.ReadAllText($@"C:\Users\Glader\Documents\Github\Booma.Server\Data\JSON\ItemPMT.Frames.json"));

			for (int i = 0; i < frameData.frames.list.Count; i++)
			{
				var entry = frameData.frames.list[i];
				var key = CreateItemTemplateKey(ItemClassType.Guard, 1, i); //Subclass 1 is Frame

				//If it's already inserted don't reinsert
				if ((await itemTemplateSet.FindAsync(key)) != null)
					continue;

				await CreateSubclassIfNeededAsync(ItemClassType.Guard, itemSubclassSet, 1, "Frame");

				var dbEntry = new DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>(ItemClassType.Guard, 1, entry.name, entry.desc, PsobbQuality.Common);
				dbEntry.TrySetPropertyValue(nameof(DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>.Id), key);
				itemTemplateSet.Add(dbEntry);
				Console.WriteLine($"Inserting: {key} - {entry.name}");
			}
		}

		private static async Task LoadWeaponEntriesAsync(DbSet<DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>> itemTemplateSet, DbSet<DBRPGSItemSubClass<ItemClassType>> itemSubclassSet)
		{
			//TODO: Remove absolute paths
			//Load weapon data
			var weaponData = JsonConvert.DeserializeObject<ItemPMTWeapons>(File.ReadAllText($@"C:\Users\Glader\Documents\Github\Booma.Server\Data\JSON\ItemPMT.Weapons.json"));

			//TODO: We're skipping SubClass 0 even though PSO does have some data in this group (for weapons its nonsense)
			for (int groupId = 1; groupId < weaponData.weapons.@group.Count; groupId++)
			{
				for (int i = 0; i < weaponData.weapons.@group[groupId].list.Count; i++)
				{
					var entry = weaponData.weapons.@group[groupId].list[i];
					var key = CreateItemTemplateKey(ItemClassType.Weapon, groupId, i);

					//If it's already inserted don't reinsert
					if ((await itemTemplateSet.FindAsync(key)) != null)
						continue;

					await CreateSubclassIfNeededAsync(ItemClassType.Weapon, itemSubclassSet, groupId);

					var dbEntry = new DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>(ItemClassType.Weapon, groupId, entry.name, entry.desc, PsobbQuality.Common);
					dbEntry.TrySetPropertyValue(nameof(DBRPGItemTemplate<ItemClassType, PsobbQuality, Vector3<byte>>.Id), key);
					itemTemplateSet.Add(dbEntry);
					Console.WriteLine($"Inserting: {key} - {entry.name}");
				}
			}
		}

		private static async Task CreateSubclassIfNeededAsync(ItemClassType @class, DbSet<DBRPGSItemSubClass<ItemClassType>> itemSubclassSet, int subclassId, string name = "TODO")
		{
			//First we must consider also that the subclass may not exist.
			if (await itemSubclassSet.FindAsync(@class, subclassId) == null)
			{
				var subclass = new DBRPGSItemSubClass<ItemClassType>(Math.Max(1, subclassId), @class, $"{name}", name);
				subclass.SetPropertyValue(nameof(DBRPGSItemSubClass<ItemClassType>.SubClassId), subclassId);
				itemSubclassSet.Add(subclass);
			}
		}

		private static int CreateItemTemplateKey(ItemClassType @class, int groupId, int i)
		{
			int shift = (1 * sizeof(byte) * 8);

			//Key: Class:C Group/SubClass:G Index:I 0xCCGGII
			int key = ((int)@class << shift * 2) + (groupId << shift) + i; 
			return key;
		}
	}
}
