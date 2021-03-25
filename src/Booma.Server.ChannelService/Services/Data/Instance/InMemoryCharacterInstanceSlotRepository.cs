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
	/// In-memory implementation of <see cref="ICharacterInstanceSlotRepository"/>
	/// </summary>
	public sealed class InMemoryCharacterInstanceSlotRepository : InMemorySlotRepository<CharacterInstanceSlot>, ICharacterInstanceSlotRepository
	{
		public InMemoryCharacterInstanceSlotRepository()
			: base(4)
		{
			
		}
	}
}
