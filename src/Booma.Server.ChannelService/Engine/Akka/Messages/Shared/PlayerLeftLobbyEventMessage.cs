using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Event message that is sent when a player has left the lobby.
	/// </summary>
	public sealed class PlayerLeftLobbyEventMessage : EntityActorMessage
	{
		/// <summary>
		/// The guid of the player leaving the lobby.
		/// </summary>
		public NetworkEntityGuid Entity { get; private set; }

		/// <summary>
		/// The slot they occupied in the lobby.
		/// </summary>
		public int Slot { get; private set; }

		public PlayerLeftLobbyEventMessage(NetworkEntityGuid entity, int slot)
		{
			if (slot < 0) throw new ArgumentOutOfRangeException(nameof(slot));

			Entity = entity ?? throw new ArgumentNullException(nameof(entity));
			Slot = slot;
		}
	}
}
