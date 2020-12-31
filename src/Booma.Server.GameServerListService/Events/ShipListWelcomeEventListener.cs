using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using Glader.Essentials;
using Glader;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	public sealed class ShipListWelcomeEventListener : LoginResponseSentEventListener
	{
		private IShipEntryRepository ShipRepository { get; }

		public ShipListWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			IShipEntryRepository shipRepository) 
			: base(subscriptionService)
		{
			ShipRepository = shipRepository ?? throw new ArgumentNullException(nameof(shipRepository));
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
			ShipEntry[] entries = await ShipRepository
				.RetrieveAllAsync();

			if (entries.Any())
			{
				await args.MessageContext.MessageService.SendMessageAsync(new SharedShipListEventPayload(entries.Select(s =>
				{
					return new MenuListing(new MenuItemIdentifier(0, 1), 0, s.Name);
				}).ToArray()));
			}
			else
			{
				await args.MessageContext.MessageService.SendMessageAsync(new SharedShipListEventPayload(BuildEmptyShipListMenu()));
			}
		}

		private MenuListing[] BuildEmptyShipListMenu()
		{
			return new[]
			{
				new MenuListing(new MenuItemIdentifier(0, 1), ushort.MaxValue, "None"),
				new MenuListing(new MenuItemIdentifier(0, 2), 0, "Refresh List"),
			};
		}
	}
}
