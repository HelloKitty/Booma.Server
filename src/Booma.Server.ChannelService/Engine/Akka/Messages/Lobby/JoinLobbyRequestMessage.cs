using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public enum LobbyJoinResponseCode
	{
		/// <summary>
		/// Should almost always be the case since a slot should be reserved.
		/// </summary>
		Success = GladerEssentialsModelConstants.RESPONSE_CODE_SUCCESS_VALUE,

		/// <summary>
		/// Unknown error.
		/// </summary>
		GeneralServerError = 2,

		/// <summary>
		/// Should rarely happen since a spot should be reserved.
		/// </summary>
		LobbyFull = 3,
	}

	public sealed class JoinLobbyRequestMessage : EntityActorMessage, IActorRequestMessage<LobbyJoinResponseCode>
	{
		/// <summary>
		/// The entity requesting to join the lobby.
		/// </summary>
		public NetworkEntityGuid Entity { get; private set; }

		public JoinLobbyRequestMessage(NetworkEntityGuid entity)
		{
			Entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}
	}
}
