using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Booma.Proxy;
using Glader.Essentials;
using Glader;

namespace Booma
{
	public sealed class ShipListWelcomeEventListener : LoginResponseSentEventListener
	{
		public ShipListWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService) : base(subscriptionService)
		{
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			//Do nothing on failure logins.
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
				return;

			//Client authed, send them the scrolling marquee and the ship list.
			await args.MessageContext.MessageService.SendMessageAsync(new CharacterTimestampEventPayload(String.Empty));
			await args.MessageContext.MessageService.SendMessageAsync(new SharedMarqueeScrollChangeEventPayload("Hello World, welcome to GladerServ? No that's terrible name!"));

			//Demo ship list
			await args.MessageContext.MessageService.SendMessageAsync(new SharedShipListEventPayload(BuildShipList()));

		}

		private MenuListing[] BuildShipList()
		{
			return new MenuListing[]
			{
				new MenuListing(new MenuItemIdentifier(0, 1), 0, "Dreamcast (86)"),
				new MenuListing(new MenuItemIdentifier(0, 2), 0, "Xbox (5)"),
				new MenuListing(new MenuItemIdentifier(0, 3), 0, "GameCube (234)"),
				new MenuListing(new MenuItemIdentifier(0, 4), 0, "BB (2530)"),
				new MenuListing(new MenuItemIdentifier(0, 5), 0, "PC (25)"),
			};
		}
	}
}
