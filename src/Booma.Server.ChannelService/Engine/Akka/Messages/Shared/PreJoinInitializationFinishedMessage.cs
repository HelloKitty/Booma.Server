using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Empty message that indicates to the actor that any pre-join initialization
	/// logic is finished.
	/// (Ex. Player in the process of joining a lobby has finished initializing everything required
	/// to ask the Parent actor Lobby/Game to continue with the process of joining).
	/// </summary>
	public sealed class PreJoinInitializationFinishedMessage : EntityActorMessage
	{
		public PreJoinInitializationFinishedMessage()
		{
			
		}
	}
}
