using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Request message to create an instance.
	/// </summary>
	public sealed class CreateInstanceMessage : EntityActorMessage, IActorRequestMessage<bool>
	{
		/// <summary>
		/// The id of the requested instance.
		/// (Will likely be the GroupId/PartyId in PSO)
		/// </summary>
		public int InstanceId { get; }

		public CreateInstanceMessage(int instanceId)
		{
			if (instanceId <= 0) throw new ArgumentOutOfRangeException(nameof(instanceId));

			InstanceId = instanceId;
		}
	}
}
