using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Booma
{
	public sealed class LambdaNetworkButtonScript : NetworkButtonScript
	{
		private Func<Task> InternalLambda { get; }

		public LambdaNetworkButtonScript(Func<Task> internalLambda)
		{
			InternalLambda = internalLambda ?? throw new ArgumentNullException(nameof(internalLambda));
		}

		public override async Task OnSelectionAsync(CancellationToken token = default)
		{
			await InternalLambda.Invoke();
		}
	}
}
