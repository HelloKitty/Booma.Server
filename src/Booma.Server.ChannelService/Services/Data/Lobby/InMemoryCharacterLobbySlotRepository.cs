using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Booma
{
	/// <summary>
	/// In-memory implementation of <see cref="ICharacterLobbySlotRepository"/>
	/// </summary>
	public sealed class InMemoryCharacterLobbySlotRepository : ICharacterLobbySlotRepository
	{
		//As dumb as it seems, this is actually easiest to model slot-based aware PSOBB protocol.
		//TODO: Is 15 the max lobby size? Make a constant!
		private CharacterLobbySlot[] InternalStore { get; } = new CharacterLobbySlot[15];

		/// <inheritdoc />
		public Task<bool> ContainsAsync(int key, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore[key] != null);
		}

		/// <inheritdoc />
		public Task<bool> TryCreateAsync(CharacterLobbySlot model, CancellationToken token = default)
		{
			if (InternalStore[model.Slot] != null)
				return Task.FromResult(false);

			InternalStore[model.Slot] = model;
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		public Task<CharacterLobbySlot> RetrieveAsync(int key, CancellationToken token = default, bool includeNavigationProperties = false)
		{
			return Task.FromResult(InternalStore[key]);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(int key, CancellationToken token = default)
		{
			InternalStore[key] = null;
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		public Task UpdateAsync(int key, CharacterLobbySlot model, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore[key] = model);
		}

		/// <inheritdoc />
		public Task<CharacterLobbySlot[]> RetrieveAllAsync(CancellationToken token = new CancellationToken())
		{
			//TODO: This copy is kinda shitty.
			//We should cache it!
			return Task.FromResult(InternalStore);
		}

		public Task<bool> HasAvailableSlotAsync(CancellationToken token = default)
		{
			//If any slots are null we have one available.
			return Task.FromResult(InternalStore.Any(s => s == null));
		}

		public Task<int> FirstAvailableSlotAsync(CancellationToken token = default)
		{
			//Finds first null slot.
			return Task.FromResult(Array.FindIndex(InternalStore, slot => slot == null));
		}

		public Task<bool> ContainsEntitySlotAsync(NetworkEntityGuid messageEntity, CancellationToken token = default)
		{
			if (messageEntity == null) throw new ArgumentNullException(nameof(messageEntity));

			return Task.FromResult(InternalStore.Any(slot => slot?.CharacterData?.EntityGuid == messageEntity));
		}

		public Task<CharacterLobbySlot> RetrieveAsync(NetworkEntityGuid guid, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.First(slot => slot?.CharacterData?.EntityGuid == guid));
		}

		public Task<IEnumerable<CharacterLobbySlot>> RetrieveInitializedAsync(CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.Where(slot => slot != null && slot.IsInitialized));
		}
	}
}
