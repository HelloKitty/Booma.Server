using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;

namespace Booma
{
	public sealed class NameQueryServiceBaseUrlFactory : IServiceBaseUrlFactory
	{
		public EntityType EntityTypeValue { get; }

		public DefaultServiceBaseUrlFactory DefaultUrlFactory { get; } = new DefaultServiceBaseUrlFactory();

		public NameQueryServiceBaseUrlFactory(EntityType entityTypeValue)
		{
			if(!Enum.IsDefined(typeof(EntityType), entityTypeValue)) throw new InvalidEnumArgumentException(nameof(entityTypeValue), (int)entityTypeValue, typeof(EntityType));

			EntityTypeValue = entityTypeValue;
		}

		public string Create(Uri context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//TODO: Refactor path building to NameQuery library
			return $"{DefaultUrlFactory.Create(context)}/api/{EntityTypeValue}Name";
		}
	}
}
