using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glader.Essentials;
using JetBrains.Annotations;

namespace Booma
{
	//TODO: Expirmental API, WIP
	public sealed class GameEngineFrameworkManager : IGameInitializable
	{
		private IGameInitializable[] Initializables { get; }

		private TaskCompletionSource<object> EngineInitializationCompletionSource { get; } = new TaskCompletionSource<object>();

		public Task Initialized => EngineInitializationCompletionSource.Task;

		public GameEngineFrameworkManager(IEnumerable<IGameInitializable> initializables)
		{
			if (initializables == null) throw new ArgumentNullException(nameof(initializables));
			Initializables = initializables.ToArray();
		}

		public async Task OnGameInitialized()
		{
			try
			{
				foreach(var init in Initializables)
					await init.OnGameInitialized();
			}
			catch (Exception e)
			{
				EngineInitializationCompletionSource.SetException(e);
				throw;
			}

			EngineInitializationCompletionSource.SetResult(null);
		}
	}
}
