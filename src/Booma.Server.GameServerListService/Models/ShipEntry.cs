using System;
using System.Collections.Generic;
using System.Text;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	/// <summary>
	/// Represents a simple model of a ship entry.
	/// </summary>
	public sealed class ShipEntry
	{
		/// <summary>
		/// The ship name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The ship endpoint.
		/// </summary>
		public ResolvedEndpoint Endpoint { get; }

		public ShipEntry(string name, ResolvedEndpoint endpoint)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
		}
	}
}
