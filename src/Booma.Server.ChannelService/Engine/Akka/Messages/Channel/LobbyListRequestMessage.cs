using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Message to request a lobby list.
	/// </summary>
	public sealed class LobbyListRequestMessage : EntityActorMessage, IActorRequestMessage<LobbyListResponseMessage>
	{
		/// <summary>
		/// Empty command/request.
		/// </summary>
		public LobbyListRequestMessage()
		{
			
		}
	}
}
