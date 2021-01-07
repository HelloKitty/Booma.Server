using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;

namespace Booma
{
	/// <summary>
	/// Data model that represents a character lobby slot.
	/// </summary>
	public sealed class CharacterLobbySlot
	{
		public InitialCharacterDataSnapshot CharacterData { get; }

		public int Slot { get; }

		public IActorRef Actor { get; }

		public CharacterLobbySlot(InitialCharacterDataSnapshot characterData, int slot, IActorRef actor)
		{
			if (slot < 0) throw new ArgumentOutOfRangeException(nameof(slot));
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
			Slot = slot;
			Actor = actor ?? throw new ArgumentNullException(nameof(actor));
		}
	}
}
