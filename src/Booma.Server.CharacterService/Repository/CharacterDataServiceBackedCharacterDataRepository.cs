using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Common.Logging;
using Glader.ASP.RPGCharacter;
using GladNet;

namespace Booma
{
	/// <summary>
	/// <see cref="ICharacterDataQueryService"/>-backed implementation of <see cref="ICharacterDataRepository"/>
	/// </summary>
	public sealed class CharacterDataServiceBackedCharacterDataRepository : ICharacterDataRepository
	{
		/// <summary>
		/// The service resolver for the data query service.
		/// </summary>
		private IServiceResolver<ICharacterDataQueryService> CharacterDataServiceResolver { get; }

		/// <summary>
		/// The service resolver for the character creation
		/// </summary>
		private IServiceResolver<ICharacterCreationService> CharacterCreationServiceResolver { get; }

		/// <summary>
		/// Logger.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The detailts about the session.
		/// </summary>
		private SessionDetails Details { get; }

		public CharacterDataServiceBackedCharacterDataRepository(IServiceResolver<ICharacterDataQueryService> characterDataServiceResolver,
			ILog logger,
			SessionDetails details, 
			IServiceResolver<ICharacterCreationService> characterCreationServiceResolver)
		{
			CharacterDataServiceResolver = characterDataServiceResolver ?? throw new ArgumentNullException(nameof(characterDataServiceResolver));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Details = details ?? throw new ArgumentNullException(nameof(details));
			CharacterCreationServiceResolver = characterCreationServiceResolver ?? throw new ArgumentNullException(nameof(characterCreationServiceResolver));
		}

		public async Task<bool> ContainsAsync(int slot, CancellationToken token = default)
		{
			RPGCharacterData[] characters = await LoadCharactersAsync(token);

			return characters != null && characters.Length > slot;
		}

		private async Task<RPGCharacterData[]> LoadCharactersAsync(CancellationToken token)
		{
			ServiceResolveResult<ICharacterDataQueryService> serviceResolutionResult
				= await CharacterDataServiceResolver.Create(token);

			if (!serviceResolutionResult.isAvailable)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Service unavailable: {nameof(ICharacterDataQueryService)}. Disconnecting Client: {Details.ConnectionId}");

				throw new InvalidOperationException($"Service unavailable: {nameof(ICharacterDataQueryService)}. Disconnecting Client: {Details.ConnectionId}");
			}

			//TODO: Implement some form of caching.
			RPGCharacterData[] characters = await serviceResolutionResult
				.Instance
				.RetrieveCharactersDataAsync(token);
			return characters;
		}

		public async Task<PlayerCharacterDataModel> RetrieveAsync(int slot, CancellationToken token = default)
		{
			RPGCharacterData[] characters = await LoadCharactersAsync(token);

			//TODO: Use an Object Mapper
			return this.Convert(characters[slot]);
		}

		private async Task<ICharacterCreationService> GetCharacterCreationServiceAsync(CancellationToken token = default)
		{
			ServiceResolveResult<ICharacterCreationService> serviceResolutionResult
				= await CharacterCreationServiceResolver.Create(token);

			if(!serviceResolutionResult.isAvailable)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Service unavailable: {nameof(ICharacterCreationService)}. Disconnecting Client: {Details.ConnectionId}");

				throw new InvalidOperationException($"Service unavailable: {nameof(ICharacterCreationService)}. Disconnecting Client: {Details.ConnectionId}");
			}

			return serviceResolutionResult.Instance;
		}

		public async Task<bool> CreateAsync(PlayerCharacterDataModel data, CancellationToken token = default)
		{
			ICharacterCreationService creationService = await GetCharacterCreationServiceAsync(token);
			var creationResult = await creationService.CreateCharacterAsync(new RPGCharacterCreationRequest(data.CharacterName), token);

			if (!creationResult.isSuccessful)
				if (Logger.IsWarnEnabled)
					Logger.Warn($"Failed to create character. Reason: {creationResult.ResultCode}");

			return creationResult.isSuccessful;
		}

		//TODO: use object mapper.
		private PlayerCharacterDataModel Convert(RPGCharacterData character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			return new PlayerCharacterDataModel(new CharacterProgress((uint) character.Progress.Experience, (uint) character.Progress.Level),
				String.Empty, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Viridia, CharacterClass.HUmar,
				new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), character.Entry.Name, (uint) character.Progress.PlayTime.TotalSeconds);
		}
	}
}
