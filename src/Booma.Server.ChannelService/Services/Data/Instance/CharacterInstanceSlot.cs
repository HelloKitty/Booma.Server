using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using MEAKKA;

namespace Booma
{
	public sealed class CharacterInstanceSlot : IWorldSlotDefinition
	{
		/// <inheritdoc />
		public int Slot { get; }

		public IEntityActorRef<InstanceCharacterActor> EntityActor { get; }

		/// <inheritdoc />
		public IActorRef Actor => EntityActor.Actor;

		/// <summary>
		/// Indicates if the character is initialized.
		/// (don't send messages to an uninitialized character reference)
		/// </summary>
		public bool IsInitialized { get; set; } = false;

		/// <inheritdoc />
		public NetworkEntityGuid Guid { get; }

		public CharacterInstanceSlot(int slot, IEntityActorRef<InstanceCharacterActor> actor, NetworkEntityGuid guid)
		{
			if(slot < 0) throw new ArgumentOutOfRangeException(nameof(slot));
			Slot = slot;
			EntityActor = actor ?? throw new ArgumentNullException(nameof(actor));
			Guid = guid ?? throw new ArgumentNullException(nameof(guid));
		}
	}
}
