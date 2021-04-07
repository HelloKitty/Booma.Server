using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	public sealed class BoomaServiceTypeIdentifierConverter : IServiceTypeIdentifierConverter<BoomaServiceType>
	{
		public string Create(BoomaServiceType context)
		{
			if (!Enum.IsDefined(typeof(BoomaServiceType), context)) throw new InvalidEnumArgumentException(nameof(context), (int) context, typeof(BoomaServiceType));
			return BoomaEndpointConstants.GetServiceIdentifier(context);
		}
	}
}
