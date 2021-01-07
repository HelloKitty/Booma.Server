using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Message to request a lobby list.
	/// </summary>
	public sealed class LobbyListResponseMessage : EntityActorMessage, IEnumerable<LobbyEntry>
	{
		/// <summary>
		/// The lobby entries.
		/// </summary>
		public LobbyEntry[] Lobbies { get; } = Array.Empty<LobbyEntry>();

		public LobbyListResponseMessage(LobbyEntry[] lobbies)
		{
			Lobbies = lobbies ?? throw new ArgumentNullException(nameof(lobbies));
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		public LobbyListResponseMessage()
		{
			
		}

		public IEnumerator<LobbyEntry> GetEnumerator()
		{
			return Lobbies.AsEnumerable().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Lobbies.GetEnumerator();
		}
	}
}
