using System;
using System.Collections.Generic;
using System.Text;
using Booma.Proxy;
using FreecraftCore.Serializer;
using GladNet;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="IMessageSerializer{TMessageType}"/> implementation for PSO Booma Patch server serialization
	/// of <see cref="PacketHeaderSerializationContext{TPayloadType}"/> for <see cref="PSOBBGamePacketPayloadServer"/>s.
	/// </summary>
	public class GamePacketHeaderSerializer : IMessageSerializer<PacketHeaderSerializationContext<PSOBBGamePacketPayloadServer>>
	{
		/// <inheritdoc />
		public void Serialize(PacketHeaderSerializationContext<PSOBBGamePacketPayloadServer> value, Span<byte> buffer, ref int offset)
		{
			//TODO: Serializer PSOBB game header DTO>
			//See: PSOBBPacketHeader in Booma.Proxy (shows how sizing is calculated, this is copy pasted basically).
			GenericTypePrimitiveSerializerStrategy<short>.Instance.Write((short) (value.PayloadSize + sizeof(short)), buffer, ref offset);
		}
	}
}
