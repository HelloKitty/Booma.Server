using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;
using JetBrains.Annotations;
using Pipelines.Sockets.Unofficial;

namespace Booma
{
	/// <summary>
	/// Base <see cref="ManagedSession{TPayloadReadType,TPayloadWriteType}"/> type for the Booma backend.
	/// Using <see cref="BaseTcpManagedSession{TPayloadReadType,TPayloadWriteType}"/> (TCP) networking.
	/// Requires additional <see cref="INetworkMessageDispatchingStrategy{TPayloadReadType,TPayloadWriteType}"/> and auto-dispatches messages
	/// through the dispatcher/
	/// </summary>
	/// <typeparam name="TMessageReadType">Incoming/read message type.</typeparam>
	/// <typeparam name="TMessageWriteType">Outgoing/write message type.</typeparam>
	public abstract class BaseBoomaManagedSession<TMessageReadType, TMessageWriteType> : BaseTcpManagedSession<TMessageReadType, TMessageWriteType> 
		where TMessageReadType : class where TMessageWriteType : class
	{
		/// <summary>
		/// This is the strategy that defines how messages are dispatched when received by the session.
		/// </summary>
		private INetworkMessageDispatchingStrategy<TMessageReadType, TMessageWriteType> MessageDispatcher { get; }

		/// <summary>
		/// The session message context.
		/// </summary>
		private SessionMessageContext<TMessageWriteType> CachedSessionContext { get; }

		/// <summary>
		/// The internal send service for the managed session.
		/// </summary>
		protected IMessageSendService<TMessageWriteType> SendService => CachedSessionContext.MessageService;

		protected BaseBoomaManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, 
			SessionMessageBuildingServiceContext<TMessageReadType, TMessageWriteType> messageServices,
			INetworkMessageDispatchingStrategy<TMessageReadType, TMessageWriteType> messageDispatcher)
			: base(networkOptions, connection, details, messageServices)
		{
			MessageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
			CachedSessionContext = CreateSessionContext(details);
		}

		protected BaseBoomaManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details,
			SessionMessageBuildingServiceContext<TMessageReadType, TMessageWriteType> messageServices,
			INetworkMessageDispatchingStrategy<TMessageReadType, TMessageWriteType> messageDispatcher,
			INetworkMessageInterface<TMessageReadType, TMessageWriteType> messageInterface)
			: base(networkOptions, connection, details, messageServices, messageInterface)
		{
			MessageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
			CachedSessionContext = CreateSessionContext(details);
		}

		protected BaseBoomaManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details,
			SessionMessageBuildingServiceContext<TMessageReadType, TMessageWriteType> messageServices,
			INetworkMessageDispatchingStrategy<TMessageReadType, TMessageWriteType> messageDispatcher,
			SessionMessageInterfaceServiceContext<TMessageReadType, TMessageWriteType> messageInterfaces)
			: base(networkOptions, connection, details, messageServices, messageInterfaces)
		{
			MessageDispatcher = messageDispatcher ?? throw new ArgumentNullException(nameof(messageDispatcher));
			CachedSessionContext = CreateSessionContext(details);
		}

		private SessionMessageContext<TMessageWriteType> CreateSessionContext(SessionDetails details)
		{
			if (details == null) throw new ArgumentNullException(nameof(details));

			//We build the session context 1 time because it should not change
			//Rather than inject as a dependency, we can build it in here. Really optional to inject
			//or build it in CTOR
			//Either way we build a send service and session context that captures it to provide to handling.
			IMessageSendService<TMessageWriteType> sendService = new QueueBasedMessageSendService<TMessageWriteType>(this.MessageService.OutgoingMessageQueue);
			return new SessionMessageContext<TMessageWriteType>(details, sendService, ConnectionService);
		}

		/// <inheritdoc />
		public override async Task OnNetworkMessageReceivedAsync(NetworkIncomingMessage<TMessageReadType> message, CancellationToken token = default)
		{
			//Dispatcher will route and/or handle messages incoming.
			await MessageDispatcher.DispatchNetworkMessageAsync(CachedSessionContext, message, token);
		}
	}
}
