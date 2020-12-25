using System;
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

			//Default PSOBB clients will get redirected to Port 12000 on the same IP.
			//Unless we redirect them first! Which we DO have a packet for!
			//We should immediately skip patching.
			SendService.SendMessageAsync<PatchingDoneCommandPayload>();
		}
	}
}