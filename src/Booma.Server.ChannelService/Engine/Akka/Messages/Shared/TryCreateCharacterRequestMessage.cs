using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Tells an actor that a character would like to join the System.
	/// </summary>
	public sealed class TryCreateCharacterRequestMessage : EntityActorMessage
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
