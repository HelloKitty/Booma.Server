using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;

namespace Booma
{
	public sealed class NetworkEntityGuid : ObjectGuid<EntityType>
	{
		public new static NetworkEntityGuid Empty { get; } = new NetworkEntityGuid();

		static NetworkEntityGuid()
		{
			
		}

		public NetworkEntityGuid(EntityType type, int id)
			: this(type, id, 0)
		{

		}

		public NetworkEntityGuid(EntityType type, int id, int entry)
		{
			SetObjectType(type);
			SetIdentifier(id);
			SetEntry(entry);

			//TODO: Support shards
			SetShard(0);
		}

		/// <summary>
		/// Serializer ctor.
		/// </summary>
		internal NetworkEntityGuid()
			: base(0)
		{
			
		}
	}
}
