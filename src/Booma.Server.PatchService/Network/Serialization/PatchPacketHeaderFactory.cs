using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using GladNet;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="IPacketHeaderFactory"/> PSO Booma Patch Server implementation.
	/// </summary>
	public sealed class PatchPacketHeaderFactory : IPacketHeaderFactory
	{
		/// <inheritdoc />
		public IPacketHeader Create(PacketHeaderCreationContext context)
		{
			//TODO: We should deserialize a PSOBB Patch Header DTO instead.
			short size = Unsafe.As<byte, short>(ref context.GetSpan()[0]);

			return new HeaderlessPacketHeader(size);
		}

		/// <inheritdoc />
		public bool IsHeaderReadable(in Span<byte> buffer)
		{
			return buffer.Length >= sizeof(short);
		}

		/// <inheritdoc />
		public bool IsHeaderReadable(in ReadOnlySequence<byte> buffer)
		{
			return buffer.Length >= sizeof(short);
		}

		/// <inheritdoc />
		public int ComputeHeaderSize(in ReadOnlySequence<byte> buffer)
		{
			return sizeof(short);
		}
	}
}
