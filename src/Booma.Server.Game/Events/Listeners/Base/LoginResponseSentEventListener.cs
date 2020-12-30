using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Glader;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// Simplified type signature for <see cref="EventListener{TSubscribableType}"/> for <see cref="ILoginResponseSentEventSubscribable"/>
	/// </summary>
	public abstract class LoginResponseSentEventListener : EventListenerAsync<ILoginResponseSentEventSubscribable, LoginResponseSentEventArgs>, IGameInitializable
	{
		protected LoginResponseSentEventListener(ILoginResponseSentEventSubscribable subscriptionService) 
			: base(subscriptionService)
		{

		}

		public Task OnGameInitialized()
		{
			return Task.CompletedTask;
		}
	}
}
