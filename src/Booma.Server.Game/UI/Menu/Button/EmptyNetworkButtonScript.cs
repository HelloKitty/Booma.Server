using System;
using System.Collections.Generic;
using System.Text;

namespace Booma
{
	/// <summary>
	/// Empty/default implementation of <see cref="NetworkButtonScript"/>
	/// </summary>
	public sealed class EmptyNetworkButtonScript : NetworkButtonScript
	{
		/// <summary>
		/// Empty script instance.
		/// </summary>
		public static EmptyNetworkButtonScript Instance { get; } = new EmptyNetworkButtonScript();

		//Do not remove.
		static EmptyNetworkButtonScript()
		{
			
		}

		private EmptyNetworkButtonScript()
		{
			
		}
	}
}
