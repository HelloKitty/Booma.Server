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
	/// In-memory implementation of <see cref="ILobbyEntryRepository"/>
	/// </summary>
	public sealed class InMemoryLobbyEntryRepository : ILobbyEntryRepository
	{
		private ConcurrentDictionary<int, LobbyEntry> InternalStore { get; } = new ConcurrentDictionary<int, LobbyEntry>();

		/// <inheritdoc />
		public Task<bool> ContainsAsync(int key, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.ContainsKey(key));
		}

		/// <inheritdoc />
		public Task<bool> TryCreateAsync(LobbyEntry model, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.TryAdd(model.LobbyId, model));
		}

		/// <inheritdoc />
		public Task<LobbyEntry> RetrieveAsync(int key, CancellationToken token = default, bool includeNavigationProperties = false)
		{
			return Task.FromResult(InternalStore[key]);
		}

		/// <inheritdoc />
		public Task<bool> TryDeleteAsync(int key, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.Remove(key, out var _));
		}

		/// <inheritdoc />
		public Task UpdateAsync(int key, LobbyEntry model, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore[key] = model);
		}

		/// <inheritdoc />
		public Task<LobbyEntry[]> RetrieveAllAsync(CancellationToken token = new CancellationToken())
		{
			//TODO: This copy is kinda shitty.
			//We should cache it!
			return Task.FromResult(InternalStore.Values.ToArray());
		}
	}
}
