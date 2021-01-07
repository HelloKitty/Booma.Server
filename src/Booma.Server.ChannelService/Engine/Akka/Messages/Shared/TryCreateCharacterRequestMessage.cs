using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Tells an actor that a character would like to join the System.
	/// Should respond with <see cref="ResponseModel"/> <see cref="CharacterActorCreationResponseCode"/>
	/// and <see cref="string"/> of the Actor's path.
	/// </summary>
	public sealed class TryCreateCharacterRequestMessage : EntityActorMessage, IActorRequestMessage<ResponseModel<string, CharacterActorCreationResponseCode>>
	{
		/// <summary>
		/// The input character data to use for creating
		/// the character actor.
		/// </summary>
		public InitialCharacterDataSnapshot CharacterData { get; }

		public TryCreateCharacterRequestMessage(InitialCharacterDataSnapshot characterData)
		{
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
		}
	}
}
