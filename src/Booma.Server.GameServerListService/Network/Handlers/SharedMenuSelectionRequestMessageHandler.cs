using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Network menu selection message handler. For: <see cref="SharedMenuSelectionRequestPayload"/>
	/// Should mostly dispatch menu interactions to the respective menu.
	/// </summary>
	public sealed class SharedMenuSelectionRequestMessageHandler : GameMessageHandler<SharedMenuSelectionRequestPayload>
	{
		//TODO: We need an abstraction here, to sit in front of menus so we can access them by id.
		private GameServerListNetworkedMenu ServerListMenu { get; }

		public SharedMenuSelectionRequestMessageHandler(GameServerListNetworkedMenu serverListMenu)
		{
			ServerListMenu = serverListMenu ?? throw new ArgumentNullException(nameof(serverListMenu));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, SharedMenuSelectionRequestPayload message, CancellationToken token = new CancellationToken())
		{
			await ServerListMenu.NetworkSelectionAsync(message.Selection, token);
		}
	}
}
