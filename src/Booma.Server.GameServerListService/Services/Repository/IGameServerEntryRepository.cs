using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	/// <summary>
	/// Contract for data interface for querying server entries.
	/// </summary>
	public interface IGameServerEntryRepository
	{
		/// <summary>
		/// Retrieves all server entries <see cref="ConnectionEntry"/>.
		/// </summary>
		/// <param name="token">Cancel token.</param>
		/// <returns>Non-null list of ships.</returns>
		Task<ConnectionEntry[]> RetrieveAllAsync(CancellationToken token = default);
	}
}
