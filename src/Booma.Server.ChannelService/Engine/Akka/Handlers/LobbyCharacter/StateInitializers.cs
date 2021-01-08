using System;
using System.Collections.Generic;
using System.Text;
using GladNet;
using MEAKKA;

namespace Booma.LobbyCharacter
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	public sealed class InitializeInitialCharacterDataSnapshot : InitializeStateMessageHandler<InitialCharacterDataSnapshot>
	{
		public InitializeInitialCharacterDataSnapshot(IMutableActorState<InitialCharacterDataSnapshot> mutableStateContainer) 
			: base(mutableStateContainer)
		{

		}
	}

	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	public sealed class InitializeMessageSender : InitializeStateMessageHandler<IMessageSendService<PSOBBGamePacketPayloadServer>>
	{
		public InitializeMessageSender(IMutableActorState<IMessageSendService<PSOBBGamePacketPayloadServer>> mutableStateContainer)
			: base(mutableStateContainer)
		{

		}
	}

	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	public sealed class InitializeNetworkEntityGuid : InitializeStateMessageHandler<NetworkEntityGuid>
	{
		public InitializeNetworkEntityGuid(IMutableActorState<NetworkEntityGuid> mutableStateContainer)
			: base(mutableStateContainer)
		{

		}
	}
}
