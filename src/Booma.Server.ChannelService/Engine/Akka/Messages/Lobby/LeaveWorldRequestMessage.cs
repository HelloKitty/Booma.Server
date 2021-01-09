using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class LeaveWorldRequestMessage : EntityActorMessage
	{
		/// <summary>
		/// The entity requesting to leave the world.
		/// </summary>
		public NetworkEntityGuid Entity { get; private set; }

		public LeaveWorldRequestMessage(NetworkEntityGuid entity)
		{
			Entity = entity ?? throw new ArgumentNullException(nameof(entity));
		}
	}
}
