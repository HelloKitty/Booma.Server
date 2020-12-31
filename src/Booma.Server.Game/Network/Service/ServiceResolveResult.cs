using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// Model for representing the result of a service resolve attempt.
	/// </summary>
	/// <typeparam name="TServiceType">The service type.</typeparam>
	public sealed class ServiceResolveResult<TServiceType>
		where TServiceType : class
	{
		/// <summary>
		/// Indicates if the service is available.
		/// </summary>
		public bool isAvailable { get; }

		/// <summary>
		/// Resolved instance.
		/// </summary>
		public TServiceType Instance { get; }

		/// <summary>
		/// Creates a successful resolve attempt.
		/// Instance of the service must be non-null.
		/// </summary>
		/// <param name="instance">The service instance.</param>
		public ServiceResolveResult(TServiceType instance)
		{
			isAvailable = true;
			Instance = instance ?? throw new ArgumentNullException(nameof(instance));
		}

		/// <summary>
		/// Creates a failed resolve result.
		/// </summary>
		public ServiceResolveResult()
		{
			isAvailable = false;
		}
	}
}
