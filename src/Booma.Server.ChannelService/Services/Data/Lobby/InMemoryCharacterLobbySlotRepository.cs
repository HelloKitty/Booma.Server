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
	public sealed class InMemoryCharacterLobbySlotRepository : InMemorySlotRepository<CharacterLobbySlot>, ICharacterLobbySlotRepository
	{
		public InMemoryCharacterLobbySlotRepository()
			: base(15)
		{
			
		}

		public override Task<bool> ContainsEntitySlotAsync(NetworkEntityGuid messageEntity, CancellationToken token = default)
		{
			if (messageEntity == null) throw new ArgumentNullException(nameof(messageEntity));

			return Task.FromResult(InternalStore.Any(slot => slot?.CharacterData?.EntityGuid == messageEntity));
		}

		public override Task<CharacterLobbySlot> RetrieveAsync(NetworkEntityGuid guid, CancellationToken token = default)
		{
			return Task.FromResult(InternalStore.First(slot => slot?.CharacterData?.EntityGuid == guid));
		}
	}
}
