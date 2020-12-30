using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;

namespace Booma
{
	/// <summary>
	/// Contract for type that provide a data access interface to character data.
	/// </summary>
	public interface ICharacterDataRepository
	{
		/// <summary>
		/// Indicates if a character exists mapped to the specified <see cref="slot"/>
		/// </summary>
		/// <param name="slot">The character slot.</param>
		/// <param name="token"></param>
		/// <returns>True if the character exists.</returns>
		Task<bool> ContainsAsync(int slot, CancellationToken token = default);

		/// <summary>
		/// Retrieves the character data mapped to the specified <see cref="slot"/>.
		/// </summary>
		/// <param name="slot">The character slot.</param>
		/// <param name="token"></param>
		/// <returns>True if the character exists.</returns>
		Task<PlayerCharacterDataModel> RetrieveAsync(int slot, CancellationToken token = default);
	}
}
