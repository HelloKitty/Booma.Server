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
		private IGameServerEntryRepository ShipRepository { get; }

		private GameServerListNetworkedMenu ServerListMenu { get; }

		public ShipListWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			IGameServerEntryRepository shipRepository, 
			GameServerListNetworkedMenu serverListMenu) 
			: base(subscriptionService)
		{
			ShipRepository = shipRepository ?? throw new ArgumentNullException(nameof(shipRepository));
			ServerListMenu = serverListMenu ?? throw new ArgumentNullException(nameof(serverListMenu));
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
			ConnectionEntry[] entries = await ShipRepository
				.RetrieveAllAsync();

			MenuListing[] menu = ServerListMenu.Create(entries);
			await args.MessageContext.MessageService.SendMessageAsync(new SharedShipListEventPayload(menu));
		}
	}
}
