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
	public sealed class ForwardPacketMessage : EntityActorMessage
	{
		/// <summary>
		/// Game packet to broadcast.
		/// </summary>
		public PSOBBGamePacketPayloadServer Packet { get; private set; }

		/// <summary>
		/// Entity responsible for sending.
		/// </summary>
		public NetworkEntityGuid Sender { get; private set; }

		/// <summary>
		/// The slot id of the forwarded packet.
		/// </summary>
		public byte TargetSlot { get; private set; }

		public ForwardPacketMessage(PSOBBGamePacketPayloadServer packet, NetworkEntityGuid sender, byte targetSlot)
		{
			Packet = packet ?? throw new ArgumentNullException(nameof(packet));
			Sender = sender ?? throw new ArgumentNullException(nameof(sender));
			TargetSlot = targetSlot;
		}
	}
}
