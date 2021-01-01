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
		private ChannelListNetworkedMenu ChannelListMenu { get; }

		public ChannelListWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService,
			ChannelListNetworkedMenu channelListMenu) 
			: base(subscriptionService)
		{
			ChannelListMenu = channelListMenu ?? throw new ArgumentNullException(nameof(channelListMenu));
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
				//WARNING: YOU MUST SEND BLOCK IN BLOCK## format!!
				new ConnectionEntry("BLOCK01", new ResolvedEndpoint("127.0.0.1", 12005))
			};

			MenuListing[] menu = ChannelListMenu.Create(entries);
			await args.MessageContext.MessageService.SendMessageAsync(new ShipBlockListEventPayload(menu));
		}
	}
}
