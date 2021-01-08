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
	public sealed class BroadcastGamePacketMessage : EntityActorMessage
	{
		/// <summary>
		/// Game packet to broadcast.
		/// </summary>
		public PSOBBGamePacketPayloadServer Packet { get; private set; }

		/// <summary>
		/// Entity responsible for broadcasting.
		/// </summary>
		public NetworkEntityGuid Broadcaster { get; private set; }

		public BroadcastGamePacketMessage(PSOBBGamePacketPayloadServer packet, NetworkEntityGuid broadcaster)
		{
			Packet = packet ?? throw new ArgumentNullException(nameof(packet));
			Broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
		}
	}
}
