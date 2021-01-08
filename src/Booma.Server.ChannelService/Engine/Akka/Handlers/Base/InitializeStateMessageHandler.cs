using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Generic abstract handler for <see cref="InitializeStateMessage{T}"/> that will assign the state
	/// to the provided <see cref="IMutableActorState{T}"/>.
	/// </summary>
	/// <typeparam name="T">The state to handle initialization for.</typeparam>
	public abstract class InitializeStateMessageHandler<T> : BaseActorMessageHandler<InitializeStateMessage<T>>
	{
		/// <summary>
		/// State container.
		/// </summary>
		private IMutableActorState<T> MutableStateContainer { get; }

		protected InitializeStateMessageHandler(IMutableActorState<T> mutableStateContainer)
		{
			MutableStateContainer = mutableStateContainer ?? throw new ArgumentNullException(nameof(mutableStateContainer));
		}

		public override Task HandleMessageAsync(EntityActorMessageContext context, InitializeStateMessage<T> message, CancellationToken token = default)
		{
			//Just initialize the state container.
			MutableStateContainer.Data = message.State;
			return Task.CompletedTask;
		}
	}
}
