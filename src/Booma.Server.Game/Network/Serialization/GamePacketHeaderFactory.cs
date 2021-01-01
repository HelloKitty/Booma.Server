using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Booma;
using GladNet;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="IPacketHeaderFactory"/> PSO Booma Game Server implementation.
	/// </summary>
	public sealed class GamePacketHeaderFactory : IPacketHeaderFactory
	{
		private NetworkConnectionOptions Options { get; }

		public GamePacketHeaderFactory(NetworkConnectionOptions options)
		{
			Options = options ?? throw new ArgumentNullException(nameof(options));
		}

		/// <inheritdoc />
		public IPacketHeader Create(PacketHeaderCreationContext context)
		{
			//TODO: We should deserialize a PSOBB Game Header DTO instead.
			short size = Unsafe.As<byte, short>(ref context.GetSpan()[0]);

			//PSOBB has size as FULL packet size. PSOBBPacketHeader object will provide
			//correct payload vs packet lengths.
			return new PPSOBBHeaderAdapter(new PSOBBPacketHeader(size));
		}

		/// <inheritdoc />
		public bool IsHeaderReadable(in Span<byte> buffer)
		{
			//8 is cipher blocks size.
			return buffer.Length >= 8;
		}

		/// <inheritdoc />
		public bool IsHeaderReadable(in ReadOnlySequence<byte> buffer)
		{
			//8 is cipher blocks size.
			return buffer.Length >= 8;
		}

		/// <inheritdoc />
		public int ComputeHeaderSize(in ReadOnlySequence<byte> buffer)
		{
			return 8;
		}
	}
}
