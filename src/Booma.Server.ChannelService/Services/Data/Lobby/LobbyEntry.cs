using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Simple data model for a lobby entry.
	/// </summary>
	public sealed class LobbyEntry
	{
		/// <summary>
		/// The id of the lobby entry.
		/// </summary>
		public int LobbyId { get; }

		//TODO: Should we expose this??
		/// <summary>
		/// The path to the lobby actor.
		/// </summary>
		public string LobbyActorPath { get; }

		public LobbyEntry(int lobbyId, string lobbyActorPath)
		{
			if (lobbyId < 0) throw new ArgumentOutOfRangeException(nameof(lobbyId));

			LobbyId = lobbyId;
			LobbyActorPath = lobbyActorPath ?? throw new ArgumentNullException(nameof(lobbyActorPath));
		}
	}
}
