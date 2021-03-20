using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Simple data model for a instance entry.
	/// </summary>
	public sealed class InstanceEntry
	{
		/// <summary>
		/// The id of the instance entry.
		/// </summary>
		public int InstanceId { get; }

		//TODO: Should we expose this??
		/// <summary>
		/// The path to the instance actor.
		/// </summary>
		public string InstanceActorPath { get; }

		public InstanceEntry(int instanceId, string lobbyActorPath)
		{
			if (instanceId < 0) throw new ArgumentOutOfRangeException(nameof(instanceId));

			InstanceId = instanceId;
			InstanceActorPath = lobbyActorPath ?? throw new ArgumentNullException(nameof(lobbyActorPath));
		}
	}
}
