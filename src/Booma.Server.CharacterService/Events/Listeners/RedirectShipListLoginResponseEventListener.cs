using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Booma.Proxy;
using Glader.Essentials;
using Glader;

namespace Booma
{
	/// <summary>
	/// Event listener that redirects a sessions if they're on the session stage
	/// that for ship list.
	/// </summary>
	public sealed class RedirectShipListLoginResponseEventListener : LoginResponseSentEventListener
	{
		/// <summary>
		/// Factory that can build redirectors for moving the session.
		/// </summary>
		private ISessionRedirectorFactory RedirectionFactory { get; }

		public RedirectShipListLoginResponseEventListener(ILoginResponseSentEventSubscribable subscriptionService, ISessionRedirectorFactory redirectionFactory) 
			: base(subscriptionService)
		{
			RedirectionFactory = redirectionFactory ?? throw new ArgumentNullException(nameof(redirectionFactory));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
				return;

			//Only redirect if their at the point where they're about to enter a ship and
			//need ship listing.
			if (args.Stage != SharedLoginRequest93Payload.SessionStage.PreShip)
				return;

			//TODO: Redirect to Character service on successful login on Login service.
			//Redirect the session on success
			await RedirectionFactory
				.Create()
				.RedirectAsync(BoomaServiceType.GameServerListService, args.MessageContext.MessageService);
		}
	}
}
