﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using FreecraftCore.Serializer;
using GladNet;
using Pipelines.Sockets.Unofficial;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="BaseTcpManagedSession{TPayloadReadType,TPayloadWriteType}"/> Patch Server Peer/Session implementation.
	/// </summary>
	public sealed class BoomaPatchManagedSession : BaseBoomaManagedSession<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>
	{
		public BoomaPatchManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, SessionMessageBuildingServiceContext<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer> messageServices, 
			INetworkMessageDispatchingStrategy<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer> messageDispatcher)
			: base(networkOptions, connection, details, messageServices, messageDispatcher)
		{

		}

		/// <inheritdoc />
		protected override void OnSessionInitialized()
		{
			base.OnSessionInitialized();

			//We should immediately send a hello for testing and then skip patching.
			SendService.SendMessageAsync(new PatchingMessagePayload("Hello world!"));
			SendService.SendMessageAsync(new PatchingDoneCommandPayload());
		}
	}
}