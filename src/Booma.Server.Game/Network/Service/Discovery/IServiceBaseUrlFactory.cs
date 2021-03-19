using System.Collections.Generic;
using System.Text;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using JetBrains.Annotations;

namespace Booma
{
	public interface IServiceBaseUrlFactory : IFactoryCreatable<string, ResolvedEndpoint>
	{

	}
}
