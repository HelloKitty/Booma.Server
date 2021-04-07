using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

		public CharacterDataSnapshotFactory(IServiceResolver<IPSOBBCharacterDataQueryService> dataQueryService, 
			ICharacterOptionsConfigurationFactory optionsFactory, 
			IServiceResolver<IPSOBBCharacterAppearanceService> appearanceService)
		{
			DataQueryService = dataQueryService ?? throw new ArgumentNullException(nameof(dataQueryService));
			OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
			AppearanceService = appearanceService ?? throw new ArgumentNullException(nameof(appearanceService));
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

			//new InitializeCharacterDataEventPayload(new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), CreateDefaultCharacterData(), 0, new CharacterBankData(0, Enumerable.Repeat(new BankItem(), 200).ToArray()), new GuildCardEntry(1, "Glader", String.Empty, String.Empty, 1, SectionId.Viridia, CharacterClass.HUmar), 0, configuration)
			return new InitialCharacterDataSnapshot(CreateEmptyInventory(), CreateEmptyBank(), CreateEmptyStats(), new CharacterProgress((uint) data.Progress.Experience, (uint) data.Progress.Level),
				new CharacterSpecialCustomInfo(0, modelType, 0), new CharacterVersionData(0, 0, 0), customizationInfo,
				new GuildCardEntry(1, data.Entry.Name, String.Empty, String.Empty, 1, SectionId.Viridia, data.ClassType), configuration, guid);
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

		private CharacterInventoryData CreateEmptyInventory()
		{
			return new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray());
		}

		//TODO: use object mapper.
		private LobbyCharacterData Convert(RPGCharacterData<CharacterRace, CharacterClass> character)
		{
			if(character == null) throw new ArgumentNullException(nameof(character));

			return new LobbyCharacterData(CreateEmptyStats(), 0, 0, new CharacterProgress((uint)character.Progress.Experience, (uint)character.Progress.Level), 0, String.Empty, 0, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Viridia, character.ClassType,
				new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), character.Entry.Name);
		}

		private static CharacterStats CreateEmptyStats()
		{
			return new CharacterStats(Enumerable.Repeat((ushort)1, 7).ToArray());
		}
	}
}
