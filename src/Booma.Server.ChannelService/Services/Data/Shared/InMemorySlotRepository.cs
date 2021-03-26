using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Booma
{
	public abstract class InMemorySlotRepository<TSlotType> : IBaseSlotRepository<TSlotType> 
		where TSlotType : IWorldSlotDefinition
	{
		//As dumb as it seems, this is actually easiest to model slot-based aware PSOBB protocol.
		protected TSlotType[] InternalStore { get; }

		protected InMemorySlotRepository(int maximumSlotCount)
		{
			if (maximumSlotCount <= 0) throw new ArgumentOutOfRangeException(nameof(maximumSlotCount));

			InternalStore = new TSlotType[maximumSlotCount];
		}

		/// <inheritdoc />
		public Task<bool> ContainsAsync(int key, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore[key] != null);
		}

		/// <inheritdoc />
		public Task<bool> TryCreateAsync(TSlotType model, CancellationToken token = default)
		{
			if (InternalStore[model.Slot] != null)
				return Task.FromResult(false);

			InternalStore[model.Slot] = model;
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		public Task<TSlotType> RetrieveAsync(int key, CancellationToken token = default, bool includeNavigationProperties = false)
		{
			return Task.FromResult(InternalStore[key]);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(int key, CancellationToken token = default)
		{
			InternalStore[key] = default;
			return Task.FromResult(true);
		}

		/// <inheritdoc />
		public Task UpdateAsync(int key, TSlotType model, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore[key] = model);
		}

		/// <inheritdoc />
		public Task<TSlotType[]> RetrieveAllAsync(CancellationToken token = default)
		{
			//TODO: This copy is kinda shitty.
			//We should cache it!
			return Task.FromResult(InternalStore);
		}

		/// <inheritdoc />
		public Task<bool> HasAvailableSlotAsync(CancellationToken token = default)
		{
			//If any slots are null we have one available.
			return Task.FromResult(InternalStore.Any(s => s == null));
		}

		/// <inheritdoc />
		public Task<int> FirstAvailableSlotAsync(CancellationToken token = default)
		{
			//Finds first null slot.
			return Task.FromResult(Array.FindIndex(InternalStore, slot => slot == null));
		}

		/// <inheritdoc />
		public Task<bool> ContainsEntitySlotAsync(NetworkEntityGuid messageEntity, CancellationToken token = default)
		{
			if(messageEntity == null) throw new ArgumentNullException(nameof(messageEntity));

			return Task.FromResult(InternalStore.Any(slot => slot.Guid == messageEntity));
		}

		/// <inheritdoc />
		public Task<TSlotType> RetrieveAsync(NetworkEntityGuid guid, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.First(slot => slot.Guid == guid));
		}

		/// <inheritdoc />
		public Task<IEnumerable<TSlotType>> RetrieveInitializedAsync(CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.Where(slot => slot != null && slot.IsInitialized));
		}

		/// <inheritdoc />
		public Task<int> RetrieveLeaderIdAsync(CancellationToken token = default)
		{
			foreach (var entry in InternalStore)
				if (entry != null)
					return Task.FromResult(entry.Slot);

			//Leader is or will be 0 slot.
			return Task.FromResult(0);
		}
	}
}
