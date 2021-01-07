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

		//TODO: Include player count.

		public LobbyEntry(int lobbyId)
		{
			if (lobbyId < 0) throw new ArgumentOutOfRangeException(nameof(lobbyId));

			LobbyId = lobbyId;
		}
	}
}
