using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;

namespace Booma
{
	/// <summary>
	/// Simplified type signature for <see cref="EventListener{TSubscribableType}"/> for <see cref="ILoginResponseSentEventSubscribable"/>
	/// </summary>
	public abstract class LoginResponseSentEventListener : EventListener<ILoginResponseSentEventSubscribable, LoginResponseSentEventArgs>
	{
		protected LoginResponseSentEventListener(ILoginResponseSentEventSubscribable subscriptionService) 
			: base(subscriptionService)
		{

		}
	}
}
