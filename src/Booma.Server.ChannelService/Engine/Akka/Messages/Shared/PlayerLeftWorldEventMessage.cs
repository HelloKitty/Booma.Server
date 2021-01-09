using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Event message that is sent when a player has left the world.
	/// </summary>
	public sealed class PlayerLeftWorldEventMessage : EntityActorMessage
	{
		/// <summary>
		/// The guid of the player leaving the world.
		/// </summary>
		public NetworkEntityGuid Entity { get; private set; }

		/// <summary>
		/// The slot they occupied in the world.
		/// (This is PSOBB specific)
		/// </summary>
		public int Slot { get; private set; }

		public PlayerLeftWorldEventMessage(NetworkEntityGuid entity, int slot)
		{
			if (slot < 0) throw new ArgumentOutOfRangeException(nameof(slot));

			Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			Slot = slot;
		}
	}
}
