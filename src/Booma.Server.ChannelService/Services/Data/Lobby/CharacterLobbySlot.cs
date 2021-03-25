using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Data model that represents a character lobby slot.
	/// </summary>
	public sealed class CharacterLobbySlot : IWorldSlotDefinition
	{
		public InitialCharacterDataSnapshot CharacterData { get; }

		public int Slot { get; }

		public IEntityActorRef<LobbyCharacterActor> EntityActor { get; }

		public IActorRef Actor => EntityActor.Actor;

		/// <summary>
		/// Indicates if the character is initialized.
		/// (don't send messages to an uninitialized character reference)
		/// </summary>
		public bool IsInitialized { get; set; } = false;

		public NetworkEntityGuid Guid => CharacterData?.EntityGuid ?? NetworkEntityGuid.Empty;

		public CharacterLobbySlot(InitialCharacterDataSnapshot characterData, int slot, IEntityActorRef<LobbyCharacterActor> actor)
		{
			if (slot < 0) throw new ArgumentOutOfRangeException(nameof(slot));
			CharacterData = characterData ?? throw new ArgumentNullException(nameof(characterData));
			Slot = slot;
			EntityActor = actor ?? throw new ArgumentNullException(nameof(actor));
		}
	}
}
