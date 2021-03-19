using System;
using System.Collections.Generic;
using System.Text;
using Glader.ASP.NameQuery;
using Glader.Essentials;

namespace Booma
{
	public interface IBoomaEntityNameDictionary : IEntityNameDictionary<NetworkEntityGuid, EntityType>
	{

	}

	public sealed class BoomaEntityNameDictionary : CachedEntityNameDictionary<NetworkEntityGuid, EntityType>, IBoomaEntityNameDictionary
	{

	}
}
