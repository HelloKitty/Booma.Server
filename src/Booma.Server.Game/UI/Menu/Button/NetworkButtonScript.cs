using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Booma
{
	/// <summary>
	/// Base type for menu button scripts.
	/// </summary>
	public abstract class NetworkButtonScript
	{
		/// <summary>
		/// Invokes when the client selects a menu entry button.
		/// </summary>
		/// <returns>Awaitable.</returns>
		public virtual Task OnSelectionAsync(CancellationToken token = default)
		{
			return Task.CompletedTask;
		}
	}
}
