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

		/// <summary>
		/// Indicates if the lobby contains an assigned slot for the provided entity.
		/// </summary>
		/// <param name="messageEntity">Entity to look for.</param>
		/// <param name="token">The cancel token.</param>
		/// <returns>True if the lobby contains a slot assigned to the entity.</returns>
		Task<bool> ContainsEntitySlotAsync(NetworkEntityGuid messageEntity, CancellationToken token = default);

		/// <summary>
		/// Retrieves a lobby entry by the <see cref="NetworkEntityGuid"/>.
		/// </summary>
		/// <param name="guid">The entity guid.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns></returns>
		Task<CharacterLobbySlot> RetrieveAsync(NetworkEntityGuid guid, CancellationToken token = default);

	}
}
