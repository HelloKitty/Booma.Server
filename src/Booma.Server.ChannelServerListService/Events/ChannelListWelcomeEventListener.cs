using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Glader.Essentials;
using Glader;
using Glader.ASP.ServiceDiscovery;

namespace Booma
{
	public sealed class ChannelListWelcomeEventListener : LoginResponseSentEventListener
	{
		private GameServerListNetworkedMenu ServerListMenu { get; }

		public ChannelListWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService,
			GameServerListNetworkedMenu serverListMenu) 
			: base(subscriptionService)
		{
			ServerListMenu = serverListMenu ?? throw new ArgumentNullException(nameof(serverListMenu));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			//Do nothing on failure logins.
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
				return;

			//TODO: This is demo channel handling, we need REAL service discovery for localized server channels.
			//Channel list.
			ConnectionEntry[] entries = new ConnectionEntry[1]
			{
				new ConnectionEntry("Block 01", new ResolvedEndpoint("127.0.0.1", 12005))
			};

			MenuListing[] menu = ServerListMenu.Create(entries);
			await args.MessageContext.MessageService.SendMessageAsync(new ShipBlockListEventPayload(menu));
		}
	}
}
