using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class LeaveLobbyRequestMessage : EntityActorMessage
	{
		/// <summary>
		/// The entity requesting to leave the lobby.
		/// </summary>
		public NetworkEntityGuid Entity { get; private set; }

		public LeaveLobbyRequestMessage(NetworkEntityGuid entity)
		{
			Entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}
	}
}
