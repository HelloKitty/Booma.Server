using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
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
		/// Logger.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The detailts about the session.
		/// </summary>
		private SessionDetails Details { get; }

		public CharacterDataServiceBackedCharacterDataRepository(IServiceResolver<ICharacterDataQueryService> characterDataServiceResolver,
			ILog logger,
			SessionDetails details)
		{
			CharacterDataServiceResolver = characterDataServiceResolver ?? throw new ArgumentNullException(nameof(characterDataServiceResolver));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			Details = details ?? throw new ArgumentNullException(nameof(details));
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
