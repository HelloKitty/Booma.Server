using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;

namespace Booma
{
	public interface ICharacterLobbySlotRepository : IGenericRepositoryCrudable<int, CharacterLobbySlot>, IEntireTableQueryable<CharacterLobbySlot>
	{
		/// <summary>
		/// Indicates if a slot is available in the lobby.
		/// </summary>
		/// <param name="token">Cancel token.</param>
		/// <returns>True if a slot is available.</returns>
		Task<bool> HasAvailableSlotAsync(CancellationToken token = default);

		/// <summary>
		/// Indicates the first available slot value for the character lobby slot.
		/// </summary>
		/// <param name="token">Cancel token.</param>
		/// <returns>Available slot.</returns>
		Task<int> FirstAvailableSlotAsync(CancellationToken token = default);
	}
}
