using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Booma.Proxy;
using GladNet;

namespace Booma
{
	//TODO: The networked menu API is a WIP. May change a lot!
	/// <summary>
	/// Networked menu for the ship list.
	/// </summary>
	public sealed class GameServerListNetworkedMenu : BaseNetworkedMenu<GameServerListMenuCode, ShipEntry[]>
	{
		private IMessageSendService<PSOBBGamePacketPayloadServer> SendService { get; }

		public GameServerListNetworkedMenu(IMessageSendService<PSOBBGamePacketPayloadServer> sendService) 
			: base(GameServerListMenuCode.ServerListMenu)
		{
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}

		protected override NetworkedButtonMenuEntry[] CreateMenu(ShipEntry[] context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			return CreateServerListMenuItems(context)
				.ToArray();
		}

		private IEnumerable<NetworkedButtonMenuEntry> CreateServerListMenuItems(ShipEntry[] context)
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
