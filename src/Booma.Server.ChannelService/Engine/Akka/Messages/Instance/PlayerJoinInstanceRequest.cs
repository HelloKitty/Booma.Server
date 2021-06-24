using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public enum PlayerJoinInstanceResponseCode
	{
		Success = GladerEssentialsModelConstants.RESPONSE_CODE_SUCCESS_VALUE,
		GeneralServerError = 2,
		InstanceFull = 3,
		NotAuthorized = 4,
	}

	/// <summary>
	/// Actor message sent by a player to join an instance.
	/// Response returns <see cref="PlayerJoinInstanceResponseCode"/> and the player's created Actor path.
	/// </summary>
	public sealed class PlayerJoinInstanceRequest : EntityActorMessage, IActorRequestMessage<ResponseModel<string, PlayerJoinInstanceResponseCode>>
	{
		/// <summary>
		/// The guid of the player trying to join.
		/// </summary>
		public NetworkEntityGuid PlayerGuid { get; init; }

		/// <summary>
		/// The input character data to use for creating
		/// the character actor.
		/// </summary>
		public InitialCharacterDataSnapshot CharacterData { get; init; }
	}
}
