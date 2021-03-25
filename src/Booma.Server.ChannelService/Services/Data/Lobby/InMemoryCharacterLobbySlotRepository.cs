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
	}
}
