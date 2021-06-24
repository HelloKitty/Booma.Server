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
	public sealed class ServerListLoginEventListener : LoginResponseSentEventListener
	{
		private IGameServerEntryRepository ServerRepository { get; }

		private GameServerListNetworkedMenu ServerListMenu { get; }

		public ServerListLoginEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			IGameServerEntryRepository serverRepository, 
			GameServerListNetworkedMenu serverListMenu) 
			: base(subscriptionService)
		{
			ServerRepository = serverRepository ?? throw new ArgumentNullException(nameof(serverRepository));
			ServerListMenu = serverListMenu ?? throw new ArgumentNullException(nameof(serverListMenu));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			//Do nothing on failure logins.
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
				return;

			//TODO: Put into a strategy
			//Compute the actual real time
			//See Sylverant TIMESTAMP_TYPE
			var time = DateTime.UtcNow;
			string timestamp = $"{time.Year}:{time.Month:D2}:{time.Day:D2}: {time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}.{time.Millisecond:D3}";

			//Client authed, send them the scrolling marquee and the server list.
			await args.MessageContext.MessageService.SendMessageAsync(new CharacterTimestampEventPayload(timestamp));
			await args.MessageContext.MessageService.SendMessageAsync(new SharedMarqueeScrollChangeEventPayload("Hello World, welcome to GladerServ? No that's terrible name!"));

			//Demo ship list
			ConnectionEntry[] entries = await ServerRepository
				.RetrieveAllAsync();

			MenuListing[] menu = ServerListMenu.Create(entries);
			await args.MessageContext.MessageService.SendMessageAsync(new SharedShipListEventPayload(menu));
		}
	}
}
