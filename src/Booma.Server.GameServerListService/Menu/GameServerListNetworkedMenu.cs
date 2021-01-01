using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booma;
using Booma.UI;
using GladNet;

namespace Booma
{
	//TODO: The networked menu API is a WIP. May change a lot!
	/// <summary>
	/// Networked menu for the ship list.
	/// </summary>
	public class GameServerListNetworkedMenu : BaseNetworkedMenu<KnownMenuIdentifier, ConnectionEntry[]>
	{
		private IMessageSendService<PSOBBGamePacketPayloadServer> SendService { get; }

		public GameServerListNetworkedMenu(IMessageSendService<PSOBBGamePacketPayloadServer> sendService) 
			: base(KnownMenuIdentifier.SHIP)
		{
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}

		public GameServerListNetworkedMenu(KnownMenuIdentifier menu, 
			IMessageSendService<PSOBBGamePacketPayloadServer> sendService) 
			: base(menu)
		{
			if (!Enum.IsDefined(typeof(KnownMenuIdentifier), menu)) throw new ArgumentOutOfRangeException(nameof(menu), "Value should be defined in the KnownMenuIdentifier enum.");
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}

		protected override NetworkedButtonMenuEntry[] CreateMenu(ConnectionEntry[] context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return CreateServerListMenuItems(context)
				.ToArray();
		}

		private IEnumerable<NetworkedButtonMenuEntry> CreateServerListMenuItems(ConnectionEntry[] context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			foreach (var ship in context)
			{
				MenuListing menuEntry = CreateMenuItem(ship.Name);

				//TODO: Extract this to a redirection button script.
				yield return new NetworkedButtonMenuEntry(menuEntry, new LambdaNetworkButtonScript(async () =>
				{
					await SendService.SendMessageAsync(new SharedConnectionRedirectPayload(ship.Endpoint.Address, (short)ship.Endpoint.Port));
				}));
			}
		}
	}
}
