using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Empty message indicating the owner of an actor is disposing of itself
	/// and the actor.
	/// </summary>
	public sealed class ActorOwnerDisposedMessage : EntityActorMessage, IActorRequestMessage<bool>
	{
		/// <summary>
		/// Empty message indicating the owner of an actor is disposing of itself
		/// and the actor.
		/// </summary>
		public ActorOwnerDisposedMessage()
		{
			
		}
	}
}
