using System;
using System.Collections.Generic;
using System.Text;
using Booma.Proxy;
using FreecraftCore.Serializer;
using GladNet;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="IMessageSerializer{TMessageType}"/> and <see cref="IMessageDeserializer{TMessageType}"/> adapter
	/// around Booma's FreecraftCore <see cref="SerializerService"/>. Providing serialization for <see cref="PSOBBPatchPacketPayloadServer"/> and
	/// <see cref="PSOBBPatchPacketPayloadClient"/>.
	/// </summary>
	public sealed class PatchPacketSerializer : IMessageSerializer<PSOBBPatchPacketPayloadServer>, IMessageDeserializer<PSOBBPatchPacketPayloadClient>
	{
		//TODO: Inject
		/// <summary>
		/// The adapted serialization service.
		/// </summary>
		private SerializerService Serializer { get; } = new SerializerService();

		public PatchPacketSerializer()
		{
			Serializer.RegisterPatchPacketSerializers();
		}

		/// <inheritdoc />
		public void Serialize([NotNull] PSOBBPatchPacketPayloadServer value, Span<byte> buffer, ref int offset)
		{
			if (value == null) throw new ArgumentNullException(nameof(value));

			Serializer.Write(value, buffer, ref offset);
		}

		/// <inheritdoc />
		public PSOBBPatchPacketPayloadClient Deserialize(Span<byte> buffer, ref int offset)
		{
			return Serializer.Read<PSOBBPatchPacketPayloadClient>(buffer, ref offset);
		}
	}
}
