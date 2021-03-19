using System;
using System.Collections.Generic;
using System.Text;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	public sealed class DefaultServiceBaseUrlFactory : IServiceBaseUrlFactory
	{
		public string Create(ResolvedEndpoint context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return $"{context.Address}:{context.Port}";
		}
	}
}
