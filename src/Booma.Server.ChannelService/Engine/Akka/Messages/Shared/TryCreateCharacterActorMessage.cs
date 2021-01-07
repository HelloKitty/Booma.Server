using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Tells an actor that a character would like to join the System.
	/// </summary>
	public sealed class TryCreateCharacterActorMessage : EntityActorMessage
	{
		/// <summary>
		/// The input character data to use for creating
		/// the character actor.
		/// </summary>
		public InitialCharacterDataSnapshot CharacterData { get; }

		public TryCreateCharacterActorMessage(InitialCharacterDataSnapshot characterData)
		{
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
		}
	}
}
