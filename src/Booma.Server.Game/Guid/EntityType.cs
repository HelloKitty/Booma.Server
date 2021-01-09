using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	//We use the default values from WoW for now.
	/// <summary>
	/// Enumeration of entity types.
	/// </summary>
	public enum EntityType : byte
	{
		Unknown = 0,
		Item = 1,
		Container = 2,
		Creature = 3,
		Player = 4,
		GameObject = 5,
		DynamicObject = 6,
		Corpse = 7,
		Lobby = 8
	}
}
