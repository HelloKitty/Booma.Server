﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Booma;
using Glader.Essentials;
using Glader;

namespace Booma
{
	/// <summary>
	/// Event listener that redirects a session when a successful login occurs.
	/// </summary>
	public sealed class RedirectCharacterServiceOnLoginResponseEventListener : LoginResponseSentEventListener
	{
		/// <summary>
		/// Factory that can build redirectors for moving the session.
		/// </summary>
		private ISessionRedirectorFactory RedirectionFactory { get; }

		public RedirectCharacterServiceOnLoginResponseEventListener(ILoginResponseSentEventSubscribable subscriptionService, ISessionRedirectorFactory redirectionFactory) 
			: base(subscriptionService)
		{
			RedirectionFactory = redirectionFactory ?? throw new ArgumentNullException(nameof(redirectionFactory));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
				return;

			//TODO: Redirect to Character service on successful login on Login service.
			//Redirect the session on success
			await RedirectionFactory
				.Create()
				.RedirectAsync(BoomaServiceType.CharacterService, args.MessageContext.MessageService);
		}
	}
}
