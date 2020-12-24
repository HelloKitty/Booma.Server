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
	public sealed class BoomaPatchManagedSession : BaseTcpManagedSession<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>
	{
		/// <summary>
		/// This is the strategy that defines how messages are dispatched when received by the session.
		/// </summary>
		private INetworkMessageDispatchingStrategy<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer> MessageDispatcher { get; }

		/// <summary>
		/// The session message context.
		/// </summary>
		private SessionMessageContext<PSOBBPatchPacketPayloadServer> CachedSessionContext { get; }

		public BoomaPatchManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, SessionMessageBuildingServiceContext<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer> messageServices,
			INetworkMessageDispatchingStrategy<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer> messageDispatcher)
			: base(networkOptions, connection, details, messageServices)
		{
			MessageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));

			//We build the session context 1 time because it should not change
			//Rather than inject as a dependency, we can build it in here. Really optional to inject
			//or build it in CTOR
			//Either way we build a send service and session context that captures it to provide to handling.
			IMessageSendService<PSOBBPatchPacketPayloadServer> sendService = new QueueBasedMessageSendService<PSOBBPatchPacketPayloadServer>(this.MessageService.OutgoingMessageQueue);
			CachedSessionContext = new SessionMessageContext<PSOBBPatchPacketPayloadServer>(details, sendService, ConnectionService);

			//We should immediately send a hello for testing!
			sendService.SendMessageAsync(new PatchingMessagePayload("Hello world!"));
		}

		/// <inheritdoc />
		public override async Task OnNetworkMessageReceivedAsync(NetworkIncomingMessage<PSOBBPatchPacketPayloadClient> message, CancellationToken token = default)
		{
			//Dispatcher will route and/or handle messages incoming.
			await MessageDispatcher.DispatchNetworkMessageAsync(CachedSessionContext, message, token);
		}
	}
}