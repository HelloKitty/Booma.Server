using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// Contracts for type that can resolve a service instance.
	/// Implements <see cref="IFactoryCreatable{TCreateType,TContextType}"/> for service result results.
	/// </summary>
	/// <typeparam name="TServiceInterfaceType">Type of the service.</typeparam>
	public interface IServiceResolver<TServiceInterfaceType> : IFactoryCreatable<Task<ServiceResolveResult<TServiceInterfaceType>>, CancellationToken> 
		where TServiceInterfaceType : class
	{

	}
}
