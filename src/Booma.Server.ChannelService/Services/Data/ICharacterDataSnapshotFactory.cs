﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.GameConfig;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;

//TODO: When Refit fixes: https://github.com/reactiveui/refit/issues/931 we should use closed derived Type.
using IPSOBBCharacterDataQueryService = Glader.ASP.RPG.ICharacterDataQueryService<Booma.CharacterRace, Booma.CharacterClass>;

namespace Booma
{
	public sealed class CharacterDataEventPayloadCreationContext
	{
		/// <summary>
		/// Indicates the requested slot/index of character to create the
		/// data event payload fro.
		/// </summary>
		public int Slot { get; }

		public CharacterDataEventPayloadCreationContext(int slot)
		{
			if (slot < 0) throw new ArgumentOutOfRangeException(nameof(slot));

			Slot = slot;
		}
	}

	public interface ICharacterDataSnapshotFactory 
		: IAsyncFactoryCreatable<InitialCharacterDataSnapshot, CharacterDataEventPayloadCreationContext>, IAsyncFactoryCreatable<InitialCharacterDataSnapshot, NetworkEntityGuid>
	{

	}

	public sealed class CharacterDataSnapshotFactory : ICharacterDataSnapshotFactory
	{
		private IServiceResolver<IPSOBBCharacterDataQueryService> DataQueryService { get; }

		private ICharacterOptionsConfigurationFactory OptionsFactory { get; }

		private IServiceResolver<IPSOBBCharacterAppearanceService> AppearanceService { get; }

		private IBoomaGGDBFData Data { get; }

		private IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> ConfigServiceResolver { get; }

		private IServiceResolver<ICharacterInventoryService> CharacterInventoryServiceResolver { get; }

		public CharacterDataSnapshotFactory(IServiceResolver<IPSOBBCharacterDataQueryService> dataQueryService, 
			ICharacterOptionsConfigurationFactory optionsFactory, 
			IServiceResolver<IPSOBBCharacterAppearanceService> appearanceService, 
			IBoomaGGDBFData data, 
			IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> configServiceResolver, 
			IServiceResolver<ICharacterInventoryService> characterInventoryServiceResolver)
		{
			DataQueryService = dataQueryService ?? throw new ArgumentNullException(nameof(dataQueryService));
			OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
			AppearanceService = appearanceService ?? throw new ArgumentNullException(nameof(appearanceService));
			Data = data ?? throw new ArgumentNullException(nameof(data));
			ConfigServiceResolver = configServiceResolver ?? throw new ArgumentNullException(nameof(configServiceResolver));
			CharacterInventoryServiceResolver = characterInventoryServiceResolver ?? throw new ArgumentNullException(nameof(characterInventoryServiceResolver));
		}

		public async Task<InitialCharacterDataSnapshot> Create(CharacterDataEventPayloadCreationContext context)
		{
			RPGCharacterData<CharacterRace, CharacterClass>[] characterDatas = await LoadCharactersAsync();

			//If it don't exist, we throw. Not much else we can do
			RPGCharacterData<CharacterRace, CharacterClass> data = characterDatas[context.Slot];

			var customizationData = await LoadCharacterAppearanceAsync(data.Entry.Id);

			return await BuildCharacterDataAsync(data, customizationData.Result);
		}

		public async Task<InitialCharacterDataSnapshot> Create(NetworkEntityGuid context)
		{
			RPGCharacterData<CharacterRace, CharacterClass>[] characterDatas = await LoadCharactersAsync();

			//If it don't exist, we throw. Not much else we can do
			RPGCharacterData<CharacterRace, CharacterClass> data = characterDatas
				.First(d => d.Entry.Id == context.Identifier);

			var customizationData = await LoadCharacterAppearanceAsync(data.Entry.Id);

			return await BuildCharacterDataAsync(data, customizationData.Result);
		}

		private async Task<InitialCharacterDataSnapshot> BuildCharacterDataAsync(RPGCharacterData<CharacterRace, CharacterClass> data, RPGCharacterCustomizationData<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>> customizationData)
		{
			if (data == null) throw new ArgumentNullException(nameof(data));
			if (customizationData == null) throw new ArgumentNullException(nameof(customizationData));

			CharacterOptionsConfiguration configuration = await OptionsFactory.Create(CancellationToken.None);
			NetworkEntityGuid guid = new NetworkEntityGuid(EntityType.Player, data.Entry.Id);

			CharacterModelType modelType = customizationData.SlotData.ContainsKey(PsobbCustomizationSlots.Override)
				? (CharacterModelType)customizationData.SlotData[PsobbCustomizationSlots.Override]
				: CharacterModelType.Regular;

			var hairColor = customizationData.SlotColorData.ContainsKey(PsobbCustomizationSlots.Hair)
				? customizationData.SlotColorData[PsobbCustomizationSlots.Hair]
				: new Vector3<ushort>(0, 0, 0);

			var proportions = customizationData.ProportionData.ContainsKey(PsobbProportionSlots.Default)
				? customizationData.ProportionData[PsobbProportionSlots.Default]
				: new Vector2<float>(0, 0);

			CharacterCustomizationInfo customizationInfo = new CharacterCustomizationInfo(
				GetCustomizationData(PsobbCustomizationSlots.Costume, customizationData),
				GetCustomizationData(PsobbCustomizationSlots.Skin, customizationData),
				GetCustomizationData(PsobbCustomizationSlots.Face, customizationData),
				GetCustomizationData(PsobbCustomizationSlots.Head, customizationData),
				GetCustomizationData(PsobbCustomizationSlots.Hair, customizationData),
				hairColor, proportions);

			byte[] actionBarConfig = await LoadActionBarConfigAsync();

			//new InitializeCharacterDataEventPayload(new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), CreateDefaultCharacterData(), 0, new CharacterBankData(0, Enumerable.Repeat(new BankItem(), 200).ToArray()), new GuildCardEntry(1, "Glader", String.Empty, String.Empty, 1, SectionId.Viridia, CharacterClass.HUmar), 0, configuration)
			return new InitialCharacterDataSnapshot(await CreateTestInventory(data.Entry.Id), CreateEmptyBank(), CreateStats(data.Progress.Level, data.Race, data.ClassType), new CharacterProgress((uint) data.Progress.Experience, (uint) data.Progress.Level),
				new CharacterSpecialCustomInfo(0, modelType, 0), new CharacterVersionData(0, 0, 0), customizationInfo,
				new GuildCardEntry(1, data.Entry.Name, String.Empty, String.Empty, 1, SectionId.Viridia, data.ClassType), configuration, guid, actionBarConfig);
		}

		private async Task<byte[]> LoadActionBarConfigAsync()
		{
			var serviceQueryResult = await ConfigServiceResolver.Create(CancellationToken.None);

			if(!serviceQueryResult.isAvailable)
				throw new InvalidOperationException($"Failed to aquire service Type: {nameof(IGameConfigurationService<PsobbGameConfigurationType>)}");

			var result = await serviceQueryResult.Instance.RetrieveConfigAsync(ConfigurationSourceType.Character, PsobbGameConfigurationType.ActionBar);

			//TODO: Enforce defaults better?
			//Maybe they didn't have one defined for some reason (old case during developement)
			if (!result.isSuccessful)
				return new byte[0xE8];

			return result.Result.Data;
		}

		private static ushort GetCustomizationData(PsobbCustomizationSlots slot, RPGCharacterCustomizationData<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>> data)
		{
			data.SlotData.TryGetValue(slot, out var val);
			return (ushort)val;
		}

		private async Task<RPGCharacterData<CharacterRace, CharacterClass>[]> LoadCharactersAsync()
		{
			var serviceQueryResult = await DataQueryService.Create(CancellationToken.None);

			if(!serviceQueryResult.isAvailable)
				throw new InvalidOperationException($"Failed to aquire service Type: {nameof(IPSOBBCharacterDataQueryService)}");

			RPGCharacterData<CharacterRace, CharacterClass>[] characterDatas = await serviceQueryResult
				.Instance
				.RetrieveCharactersDataAsync();
			return characterDatas;
		}

		private async Task<ResponseModel<RPGCharacterCustomizationData<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>>, CharacterDataQueryResponseCode>> LoadCharacterAppearanceAsync(int characterId)
		{
			var serviceQueryResult = await AppearanceService.Create(CancellationToken.None);

			if (!serviceQueryResult.isAvailable)
				throw new InvalidOperationException($"Failed to aquire service Type: {nameof(IPSOBBCharacterAppearanceService)}");

			var model = await serviceQueryResult
				.Instance
				.RetrieveCharacterAppearanceAsync(characterId);

			return model;
		}

		private static CharacterBankData CreateEmptyBank()
		{
			return new CharacterBankData(0, Enumerable.Repeat(new BankItem(), 200).ToArray());
		}

		private async Task<CharacterInventoryData> CreateTestInventory(int characterId)
		{
			var resolveResult = await CharacterInventoryServiceResolver.Create(CancellationToken.None);

			if (!resolveResult.isAvailable)
				throw new InvalidOperationException($"Failed to aquire service Type: {nameof(ICharacterInventoryService)}");

			var inventoryResponse = await resolveResult.Instance.RetrieveItemsAsync(characterId);

			List<InventoryItem> starterItems = new List<InventoryItem>();

			//Possible empty so don't throw unless not empty
			if (inventoryResponse.isSuccessful)
			{
				foreach (var inventoryItem in inventoryResponse.Result.Items)
				{
					//TODO: Add logging/warning doesn't exist
					if (!Data.ItemTemplate.ContainsKey(inventoryItem.TemplateId))
						continue;

					var template = Data.ItemTemplate[inventoryItem.TemplateId];

					//0x00010000 is Saber id
					var item = new InventoryItem((uint) inventoryItem.InstanceId, 0, 0, 0);
					item.SetWeaponType((byte)template.SubClassId);
					item.ItemData1[0] = (byte)((int)template.ClassId & 0xFF);
					item.ItemData1[2] = (byte)(template.Id & 0xFF);

					starterItems.Add(item);
				}
			}
			else if(inventoryResponse.ResultCode != CharacterItemInventoryQueryResult.Empty)
				throw new InvalidOperationException($"Failed to load inventory for Character: {characterId} Reason: {inventoryResponse.ResultCode}");

			InventoryItem[] emptyItems = Enumerable.Repeat(new InventoryItem(), 30 - starterItems.Count).ToArray();

			return new CharacterInventoryData((byte)starterItems.Count, 0, 0, 1, starterItems.Concat(emptyItems).ToArray());
		}

		private CharacterStats CreateStats(int level, CharacterRace race, CharacterClass @class)
		{
			var stats = new ushort[7];

			//Level 1 is represented as 0
			foreach(var entry in Data.CharacterStatDefault.CalculateBaseStats(level + 1, race, @class))
				stats[(int)entry.Key] = (ushort)entry.Value.Value;

			return new CharacterStats(stats);
		}
	}
}
