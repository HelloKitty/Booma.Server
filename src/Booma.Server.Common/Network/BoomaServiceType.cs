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

		/// <summary>
		/// Service for listing the available gameservers/ships.
		/// </summary>
		GameServerListService = 4,

		/// <summary>
		/// The service responsible for maintaining and providing Character Data.
		/// </summary>
		CharacterDataService = 5,

		/// <summary>
		/// The ship service type.
		/// </summary>
		ShipService = 6,
	}
}
