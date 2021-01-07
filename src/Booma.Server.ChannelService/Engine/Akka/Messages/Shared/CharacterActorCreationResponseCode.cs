using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public enum CharacterActorCreationResponseCode
	{
		Success = GladerEssentialsModelConstants.RESPONSE_CODE_SUCCESS_VALUE,

		GeneralError = 2,

		/// <summary>
		/// Indicates there is no space to create a character actor.
		/// </summary>
		UnavailableSpace = 3,
	}
}
