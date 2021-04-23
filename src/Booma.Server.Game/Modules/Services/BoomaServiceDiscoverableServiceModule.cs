using System;
using System.Collections.Generic;
using System.Text;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	/// <inheritdoc />
	public sealed class BoomaServiceDiscoverableServiceModule<TServiceType> : ServiceDiscoverableServiceModule<TServiceType, BoomaServiceType> 
		where TServiceType : class
	{
		public BoomaServiceDiscoverableServiceModule(BoomaServiceType serviceType) 
			: base(serviceType)
		{
			ServiceNameConverter = new BoomaServiceTypeIdentifierConverter();

			//Oddly "Authorized" is our default due to legacy reasons.
			Mode = ServiceDiscoveryModuleMode.Authorized;
		}
	}
}
