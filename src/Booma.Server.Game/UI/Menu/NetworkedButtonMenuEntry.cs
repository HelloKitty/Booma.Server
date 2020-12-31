using System;
using System.Collections.Generic;
using System.Text;
using Booma.Proxy;

namespace Booma
{
	//TODO: Doc
	/// <summary>
	/// 
	/// </summary>
	public sealed class NetworkedButtonMenuEntry
	{
		/// <summary>
		/// 
		/// </summary>
		public MenuListing Entry { get; }

		/// <summary>
		/// 
		/// </summary>
		public NetworkButtonScript Script { get; }

		public NetworkedButtonMenuEntry(MenuListing entry, NetworkButtonScript script)
		{
			Entry = entry ?? throw new ArgumentNullException(nameof(entry));
			Script = script ?? throw new ArgumentNullException(nameof(script));
		}

		public NetworkedButtonMenuEntry(MenuListing entry)
			: this(entry, EmptyNetworkButtonScript.Instance)
		{
			Entry = entry;
		}
	}
}
