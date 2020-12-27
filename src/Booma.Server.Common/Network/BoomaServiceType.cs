using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Enumerations of all Booma service types.
	/// </summary>
	public enum BoomaServiceType
	{
		/// <summary>
		/// The patch service/server.
		/// </summary>
		PatchService = 0,

		/// <summary>
		/// The login/entry point service/server.
		/// </summary>
		LoginService = 1,

		/// <summary>
		/// The Authentication service.
		/// </summary>
		AuthService = 2,

		/// <summary>
		/// The character service.
		/// </summary>
		CharacterService = 3,
	}
}
