using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Booma;
using Booma.UI;
using GladNet;

namespace Booma
{
	//TODO: The networked menu API is a WIP. May change a lot!
	/// <summary>
	/// Networked menu for the channel/block list.
	/// </summary>
	public class ChannelListNetworkedMenu : GameServerListNetworkedMenu
	{
		public ChannelListNetworkedMenu(IMessageSendService<PSOBBGamePacketPayloadServer> sendService) 
			: base(KnownMenuIdentifier.BLOCK, sendService)
		{

		}
	}
}
