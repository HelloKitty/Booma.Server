using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	public static class CharacterClassExtensions
	{
		public static CharacterRace ToRace(this CharacterClass @class)
		{
			switch (@class)
			{
				case CharacterClass.HUmar:
					return CharacterRace.Human;
				case CharacterClass.HUnewearl:
					return CharacterRace.Newman;
				case CharacterClass.HUcast:
					return CharacterRace.Cast;
				case CharacterClass.RAmar:
					return CharacterRace.Human;
				case CharacterClass.RAcast:
					return CharacterRace.Cast;
				case CharacterClass.RAcaseal:
					return CharacterRace.Cast;
				case CharacterClass.FOmarl:
					return CharacterRace.Human;
				case CharacterClass.FOnewm:
					return CharacterRace.Newman;
				case CharacterClass.FOnewearl:
					return CharacterRace.Newman;
				case CharacterClass.HUcaseal:
					return CharacterRace.Cast;
				case CharacterClass.FOmar:
					return CharacterRace.Human;
				case CharacterClass.RAmarl:
					return CharacterRace.Human;
				default:
					throw new ArgumentOutOfRangeException(nameof(@class), @class, null);
			}
		}
	}
}
