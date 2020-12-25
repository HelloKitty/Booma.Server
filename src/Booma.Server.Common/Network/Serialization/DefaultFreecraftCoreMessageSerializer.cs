using System;
using System.Collections.Generic;
using System.Text;
using FreecraftCore.Serializer;
using GladNet;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// Default FreecraftCore implementation of GladNet's <see cref="IMessageSerializer{TMessageType}"/> and <see cref="IMessageDeserializer{TMessageType}"/> adapter
	/// around Booma's FreecraftCore <see cref="SerializerService"/>. Providing serialization for <typeparamref name="TMessageReadType"/> and
	/// <typeparamref name="TMessageWriteType"/>.
	/// </summary>
	public sealed class DefaultFreecraftCoreMessageSerializer<TMessageReadType, TMessageWriteType> : IMessageDeserializer<TMessageReadType>, IMessageSerializer<TMessageWriteType> 
		where TMessageReadType : class, ITypeSerializerReadingStrategy<TMessageReadType>
		where TMessageWriteType : class, ITypeSerializerWritingStrategy<TMessageWriteType>
	{
		/// <summary>
		/// The adapted serialization service.
		/// </summary>
		private ISerializerService Serializer { get; }

		public DefaultFreecraftCoreMessageSerializer([NotNull] ISerializerService serializer)
		{
			Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
		}

		/// <inheritdoc />
		public void Serialize([NotNull] TMessageWriteType value, Span<byte> buffer, ref int offset)
		{
			if(value == null) throw new ArgumentNullException(nameof(value));

			Serializer.Write(value, buffer, ref offset);
		}

		/// <inheritdoc />
		public TMessageReadType Deserialize(Span<byte> buffer, ref int offset)
		{
			return Serializer.Read<TMessageReadType>(buffer, ref offset);
		}
	}
}
