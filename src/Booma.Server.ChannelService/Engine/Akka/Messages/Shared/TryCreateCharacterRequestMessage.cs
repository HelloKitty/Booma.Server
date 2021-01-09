using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public enum CharacterActorCreationResponseCode
	{
		Success = GladerEssentialsModelConstants.RESPONSE_CODE_SUCCESS_VALUE,

		GeneralError = 2,

		/// <summary>
		/// Indicates there is no space to create a character actor.
		/// </summary>
		UnavailableSpace = 3,
	}

	/// <summary>
	/// Tells an actor that a character would like to join the System.
	/// Should respond with <see cref="ResponseModel{TModelType,TResponseCodeType}"/> <see cref="CharacterActorCreationResponseCode"/>
	/// and <see cref="string"/> of the Actor's path.
	/// </summary>
	public sealed class TryCreateCharacterRequestMessage : EntityActorMessage, IActorRequestMessage<ResponseModel<string, CharacterActorCreationResponseCode>>
	{
		/// <summary>
		/// The input character data to use for creating
		/// the character actor.
		/// </summary>
		public InitialCharacterDataSnapshot CharacterData { get; }

		/// <summary>
		/// Requested lobby id.
		/// </summary>
		public int LobbyId { get; }

		public TryCreateCharacterRequestMessage(InitialCharacterDataSnapshot characterData, int lobbyId)
		{
			if (lobbyId < 0) throw new ArgumentOutOfRangeException(nameof(lobbyId));

			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
			LobbyId = lobbyId;
		}
	}
}
