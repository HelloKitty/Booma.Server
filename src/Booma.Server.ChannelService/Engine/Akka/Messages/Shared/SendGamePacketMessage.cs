using System;
using System.Collections.Generic;
using System.Text;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Simple message type that acts as an envelope to send a <see cref="PSOBBGamePacketPayloadServer"/> to a
	/// target actor's network interface.
	/// </summary>
	public sealed class SendGamePacketMessage : EntityActorMessage
	{
		/// <summary>
		/// Game packet to send to the actor's client.
		/// </summary>
		public PSOBBGamePacketPayloadServer Packet { get; private set; }

		public SendGamePacketMessage(PSOBBGamePacketPayloadServer packet)
		{
			Packet = packet ?? throw new ArgumentNullException(nameof(packet));
		}
	}
}
