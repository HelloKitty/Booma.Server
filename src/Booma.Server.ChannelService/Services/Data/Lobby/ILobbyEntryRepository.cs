using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// <see cref="IGenericRepositoryCrudable{TKey,TModel}"/> for <see cref="LobbyEntry"/>.
	/// </summary>
	public interface ILobbyEntryRepository : IGenericRepositoryCrudable<int, LobbyEntry>, IEntireTableQueryable<LobbyEntry>
	{

	}
}
