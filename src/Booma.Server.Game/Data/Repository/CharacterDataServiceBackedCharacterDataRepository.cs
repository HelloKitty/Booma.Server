using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Common.Logging;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using GladNet;

//TODO: When Refit fixes: https://github.com/reactiveui/refit/issues/931 we should use closed derived Type.
using IPSOBBCharacterCreationService = Glader.ASP.RPG.ICharacterCreationService<Booma.CharacterRace, Booma.CharacterClass>;
using IPSOBBCharacterDataQueryService = Glader.ASP.RPG.ICharacterDataQueryService<Booma.CharacterRace, Booma.CharacterClass>;

namespace Booma
{
	/// <summary>
	/// <see cref="ICharacterDataQueryService{TRaceType,TClassType}"/>-backed implementation of <see cref="ICharacterDataRepository"/>
	/// </summary>
	public sealed class CharacterDataServiceBackedCharacterDataRepository : ICharacterDataRepository
	{
		/// <summary>
		/// The service resolver for the data query service.
		/// </summary>
		private IServiceResolver<ICharacterDataQueryService<Booma.CharacterRace, CharacterClass>> CharacterDataServiceResolver { get; }

		/// <summary>
		/// The service resolver for the character creation
		/// </summary>
		private IServiceResolver<IPSOBBCharacterCreationService> CharacterCreationServiceResolver { get; }

		/// <summary>
		/// The service resolver for the character creation
		/// </summary>
		private IServiceResolver<IPSOBBCharacterAppearanceService> AppearanceServiceResolver { get; }

		/// <summary>
		/// Logger.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The detailts about the session.
		/// </summary>
		private SessionDetails Details { get; }

		public CharacterDataServiceBackedCharacterDataRepository(IServiceResolver<IPSOBBCharacterDataQueryService> characterDataServiceResolver,
			ILog logger,
			SessionDetails details, 
			IServiceResolver<IPSOBBCharacterCreationService> characterCreationServiceResolver, 
			IServiceResolver<IPSOBBCharacterAppearanceService> appearanceServiceResolver)
		{
			CharacterDataServiceResolver = characterDataServiceResolver ?? throw new ArgumentNullException(nameof(characterDataServiceResolver));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Details = details ?? throw new ArgumentNullException(nameof(details));
			CharacterCreationServiceResolver = characterCreationServiceResolver ?? throw new ArgumentNullException(nameof(characterCreationServiceResolver));
			AppearanceServiceResolver = appearanceServiceResolver ?? throw new ArgumentNullException(nameof(appearanceServiceResolver));
		}

		public async Task<bool> ContainsAsync(int slot, CancellationToken token = default)
		{
			RPGCharacterData<CharacterRace, CharacterClass>[] characters = await LoadCharactersAsync(token);

			return characters != null && characters.Length > slot;
		}

		private async Task<RPGCharacterData<CharacterRace, CharacterClass>[]> LoadCharactersAsync(CancellationToken token)
		{
			ServiceResolveResult<IPSOBBCharacterDataQueryService> serviceResolutionResult
				= await CharacterDataServiceResolver.Create(token);

			if (!serviceResolutionResult.isAvailable)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Service unavailable: {nameof(IPSOBBCharacterDataQueryService)}. Disconnecting Client: {Details.ConnectionId}");

				throw new InvalidOperationException($"Service unavailable: {nameof(IPSOBBCharacterDataQueryService)}. Disconnecting Client: {Details.ConnectionId}");
			}

			//TODO: Implement some form of caching.
			RPGCharacterData<CharacterRace, CharacterClass>[] characters = await serviceResolutionResult
				.Instance
				.RetrieveCharactersDataAsync(token);
			return characters;
		}

		public async Task<PlayerCharacterDataModel> RetrieveAsync(int slot, CancellationToken token = default)
		{
			RPGCharacterData<CharacterRace, CharacterClass>[] characters = await LoadCharactersAsync(token);

			var appearanceServiceResult = await AppearanceServiceResolver.Create(token);

			//TODO: Use an Object Mapper
			if (!appearanceServiceResult.isAvailable)
				return this.Convert(characters[slot]);
			else
			{
				//Query appearance data since the character could be customized
				var model = await appearanceServiceResult.Instance.RetrieveCharacterAppearanceAsync(characters[slot].Entry.Id, token);

				if(!model.isSuccessful)
					return this.Convert(characters[slot]);
				else
					return this.Convert(characters[slot], model.Result);
			}
		}

		private async Task<IPSOBBCharacterCreationService> GetCharacterCreationServiceAsync(CancellationToken token = default)
		{
			ServiceResolveResult<IPSOBBCharacterCreationService> serviceResolutionResult
				= await CharacterCreationServiceResolver.Create(token);

			if(!serviceResolutionResult.isAvailable)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Service unavailable: {nameof(IPSOBBCharacterCreationService)}. Disconnecting Client: {Details.ConnectionId}");

				throw new InvalidOperationException($"Service unavailable: {nameof(IPSOBBCharacterCreationService)}. Disconnecting Client: {Details.ConnectionId}");
			}

			return serviceResolutionResult.Instance;
		}

		private async Task<IPSOBBCharacterAppearanceService> GetCharacterAppearanceServiceAsync(CancellationToken token)
		{
			ServiceResolveResult<IPSOBBCharacterAppearanceService> serviceResolutionResult
				= await AppearanceServiceResolver.Create(token);

			if(!serviceResolutionResult.isAvailable)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Service unavailable: {nameof(IPSOBBCharacterAppearanceService)}. Disconnecting Client: {Details.ConnectionId}");

				throw new InvalidOperationException($"Service unavailable: {nameof(IPSOBBCharacterAppearanceService)}. Disconnecting Client: {Details.ConnectionId}");
			}

			return serviceResolutionResult.Instance;
		}

		public async Task<bool> CreateAsync(PlayerCharacterDataModel data, CancellationToken token = default)
		{
			IPSOBBCharacterCreationService creationService = await GetCharacterCreationServiceAsync(token);
			IPSOBBCharacterAppearanceService appearService = await GetCharacterAppearanceServiceAsync(token);
			var creationResult = await creationService.CreateCharacterAsync(new RPGCharacterCreationRequest<CharacterRace, CharacterClass>(data.CharacterName, ComputeCharacterRace(data.ClassRace), data.ClassRace), token);

			if (!creationResult.isSuccessful)
			{
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Failed to create character. Reason: {creationResult.ResultCode}");
			}
			else
			{
				//Character is created, let's give them an appearance
				var customizationData = new RPGCharacterCustomizationData<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>>();

				customizationData.ProportionData.Add(PsobbProportionSlots.Default, data.CustomizationInfo.Proportions);
				customizationData.SlotColorData.Add(PsobbCustomizationSlots.Hair, data.CustomizationInfo.HairColor);

				customizationData.SlotData.Add(PsobbCustomizationSlots.Hair, data.CustomizationInfo.HairId);
				customizationData.SlotData.Add(PsobbCustomizationSlots.Costume, data.CustomizationInfo.CostumeId);
				customizationData.SlotData.Add(PsobbCustomizationSlots.Face, data.CustomizationInfo.FaceId);
				customizationData.SlotData.Add(PsobbCustomizationSlots.Head, data.CustomizationInfo.HeadId);
				customizationData.SlotData.Add(PsobbCustomizationSlots.Skin, data.CustomizationInfo.SkinId);
				
				var appearanceCreationResult = await appearService.CreateCharacterAppearanceAsync(creationResult.Result.Id, customizationData, token);

				//We should create the character no matter what. They will have default customization if this fails
				if(Logger.IsWarnEnabled)
					Logger.Warn($"Failed to create character appearance for Character: {creationResult.Result.Id} Reason: {appearanceCreationResult}");
			}

			return creationResult.isSuccessful;
		}

		//TODO: Refactor to Booma.Proxy
		private CharacterRace ComputeCharacterRace(CharacterClass @class)
		{
			switch (@class)
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

		//TODO: use object mapper.
		private PlayerCharacterDataModel Convert(RPGCharacterData<CharacterRace, CharacterClass> character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			return new PlayerCharacterDataModel(new CharacterProgress((uint) character.Progress.Experience, (uint) character.Progress.Level),
				String.Empty, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Viridia, character.ClassType,
				new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), character.Entry.Name, (uint) character.Progress.PlayTime.TotalSeconds);
		}

		private PlayerCharacterDataModel Convert(RPGCharacterData<CharacterRace, CharacterClass> character, RPGCharacterCustomizationData<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>> customizationData)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));
			if (customizationData == null) throw new ArgumentNullException(nameof(customizationData));

			CharacterModelType modelType = customizationData.SlotData.ContainsKey(PsobbCustomizationSlots.Override)
				? (CharacterModelType) customizationData.SlotData[PsobbCustomizationSlots.Override]
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

			return new PlayerCharacterDataModel(new CharacterProgress((uint)character.Progress.Experience, (uint)character.Progress.Level),
				String.Empty, new CharacterSpecialCustomInfo(0, modelType, 0), SectionId.Viridia, character.ClassType,
				new CharacterVersionData(0, 0, 0), customizationInfo, character.Entry.Name, (uint)character.Progress.PlayTime.TotalSeconds);
		}

		private static ushort GetCustomizationData(PsobbCustomizationSlots slot, RPGCharacterCustomizationData<PsobbCustomizationSlots, Vector3<ushort>, PsobbProportionSlots, Vector2<float>> data)
		{
			data.SlotData.TryGetValue(slot, out var val);
			return (ushort) val;
		}
	}
}
