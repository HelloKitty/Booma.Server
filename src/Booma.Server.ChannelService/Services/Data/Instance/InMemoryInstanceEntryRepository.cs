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
	/// In-memory implementation of <see cref="IInstanceEntryRepository"/>
	/// </summary>
	public sealed class InMemoryInstanceEntryRepository : IInstanceEntryRepository
	{
		private ConcurrentDictionary<int, InstanceEntry> InternalStore { get; } = new ConcurrentDictionary<int, InstanceEntry>();

		/// <inheritdoc />
		public Task<bool> ContainsAsync(int key, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.ContainsKey(key));
		}

		/// <inheritdoc />
		public Task<bool> TryCreateAsync(InstanceEntry model, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.TryAdd(model.InstanceId, model));
		}

		/// <inheritdoc />
		public Task<InstanceEntry> RetrieveAsync(int key, CancellationToken token = default, bool includeNavigationProperties = false)
		{
			return Task.FromResult(InternalStore[key]);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(int key, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.Remove(key, out var _));
		}

		/// <inheritdoc />
		public Task UpdateAsync(int key, InstanceEntry model, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore[key] = model);
		}

		/// <inheritdoc />
		public Task<InstanceEntry[]> RetrieveAllAsync(CancellationToken token = new CancellationToken())
		{
			//TODO: This copy is kinda shitty.
			//We should cache it!
			return Task.FromResult(InternalStore.Values.ToArray());
		}
	}
}
