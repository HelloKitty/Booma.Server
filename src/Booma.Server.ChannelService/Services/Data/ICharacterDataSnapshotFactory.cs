using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.RPGCharacter;
using Glader.Essentials;

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
		private IServiceResolver<ICharacterDataQueryService> DataQueryService { get; }

		private ICharacterOptionsConfigurationFactory OptionsFactory { get; }

		public CharacterDataSnapshotFactory(IServiceResolver<ICharacterDataQueryService> dataQueryService, 
			ICharacterOptionsConfigurationFactory optionsFactory)
		{
			DataQueryService = dataQueryService ?? throw new ArgumentNullException(nameof(dataQueryService));
			OptionsFactory = optionsFactory ?? throw new ArgumentNullException(nameof(optionsFactory));
		}

		public async Task<InitialCharacterDataSnapshot> Create(CharacterDataEventPayloadCreationContext context)
		{
			RPGCharacterData[] characterDatas = await LoadCharactersAsync();

			//If it don't exist, we throw. Not much else we can do
			RPGCharacterData data = characterDatas[context.Slot];

			return await BuildCharacterDataAsync(data);
		}

		public async Task<InitialCharacterDataSnapshot> Create(NetworkEntityGuid context)
		{
			RPGCharacterData[] characterDatas = await LoadCharactersAsync();

			//If it don't exist, we throw. Not much else we can do
			RPGCharacterData data = characterDatas
				.First(d => d.Entry.Id == context.Identifier);

			return await BuildCharacterDataAsync(data);
		}

		private async Task<InitialCharacterDataSnapshot> BuildCharacterDataAsync(RPGCharacterData data)
		{
			CharacterOptionsConfiguration configuration = await OptionsFactory.Create(CancellationToken.None);
			NetworkEntityGuid guid = new NetworkEntityGuid(EntityType.Player, data.Entry.Id);

			//new InitializeCharacterDataEventPayload(new CharacterInventoryData(0, 0, 0, 1, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), CreateDefaultCharacterData(), 0, new CharacterBankData(0, Enumerable.Repeat(new BankItem(), 200).ToArray()), new GuildCardEntry(1, "Glader", String.Empty, String.Empty, 1, SectionId.Viridia, CharacterClass.HUmar), 0, configuration)
			return new InitialCharacterDataSnapshot(CreateEmptyInventory(), CreateEmptyBank(), CreateEmptyStats(), new CharacterProgress((uint) data.Progress.Experience, (uint) data.Progress.Level),
				new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)),
				new GuildCardEntry(1, data.Entry.Name, String.Empty, String.Empty, 1, SectionId.Viridia, CharacterClass.HUmar), configuration, guid);
		}

		private async Task<RPGCharacterData[]> LoadCharactersAsync()
		{
			var serviceQueryResult = await DataQueryService.Create(CancellationToken.None);

			if (!serviceQueryResult.isAvailable)
				throw new InvalidOperationException($"Failed to aquire service Type: {nameof(ICharacterDataQueryService)}");

			RPGCharacterData[] characterDatas = await serviceQueryResult
				.Instance
				.RetrieveCharactersDataAsync();
			return characterDatas;
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
		private LobbyCharacterData Convert(RPGCharacterData character)
		{
			if(character == null) throw new ArgumentNullException(nameof(character));

			return new LobbyCharacterData(CreateEmptyStats(), 0, 0, new CharacterProgress((uint)character.Progress.Experience, (uint)character.Progress.Level), 0, String.Empty, 0, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Viridia, CharacterClass.HUmar,
				new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), character.Entry.Name);
		}

		private static CharacterStats CreateEmptyStats()
		{
			return new CharacterStats(Enumerable.Repeat((ushort)1, 7).ToArray());
		}
	}
}
