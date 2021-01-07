using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Requests an actor to create a lobby.
	/// </summary>
	public sealed class CreateLobbyMessage : EntityActorMessage
	{
		/// <summary>
		/// the lobby id to create a lobby for.
		/// </summary>
		public int LobbyId { get; }

		public CreateLobbyMessage(int lobbyId)
		{
			if (lobbyId < 0) throw new ArgumentOutOfRangeException(nameof(lobbyId));

			LobbyId = lobbyId;
		}
	}
}
