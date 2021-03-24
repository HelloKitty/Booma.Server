using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public enum WorldJoinResponseCode
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
		WorldFull = 3,
	}

	public sealed class JoinWorldRequestMessage : EntityActorMessage, IActorRequestMessage<WorldJoinResponseCode>
	{
		/// <summary>
		/// The entity requesting to join the world actor.
		/// </summary>
		public NetworkEntityGuid Entity { get; private set; }

		public JoinWorldRequestMessage(NetworkEntityGuid entity)
		{
			Entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}
	}
}
