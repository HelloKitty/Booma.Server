using System;
using System.Collections.Generic;
using System.Text;
using Booma.Proxy;
using GladNet;

namespace Booma
{
	/// <summary>
	/// PSOBB packet header adapter for the GladNet <see cref="IPacketHeader"/> interface.
	/// </summary>
	public sealed class PPSOBBHeaderAdapter : IPacketHeader
	{
		/// <inheritdoc />
		public int PacketSize => Header.PacketSize;

		/// <inheritdoc />
		public int PayloadSize => Header.PayloadSize;

		/// <summary>
		/// Adapted header instance.
		/// </summary>
		private PSOBBPacketHeader Header { get; }

		public PPSOBBHeaderAdapter([NotNull] PSOBBPacketHeader header)
		{
			Header = header ?? throw new ArgumentNullException(nameof(header));
		}
	}
}
