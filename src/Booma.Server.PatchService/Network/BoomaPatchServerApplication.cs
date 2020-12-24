using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Booma.Proxy;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="TcpGladNetServerApplication{TManagedSessionType}"/> implementation for PSO Booma Patch Service.
	/// </summary>
	public sealed class BoomaPatchServerApplication : TcpGladNetServerApplication<BoomaPatchManagedSession>
	{
		public BoomaPatchServerApplication(NetworkAddressInfo serverAddress, ILog logger)
			: base(serverAddress, logger)
		{

		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}

		/// <inheritdoc />
		public override BoomaPatchManagedSession Create(SessionCreationContext context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			//TODO: We should use AutoFac IoC/DI!
			NetworkConnectionOptions options = new NetworkConnectionOptions(2, 2, 1024);
			var serializer = new PatchPacketSerializer();

			var messageServices = new SessionMessageBuildingServiceContext<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>(new PatchPacketHeaderFactory(), serializer, serializer, new PatchPacketHeaderSerializer());

			//Build the message disaptching strategy, for how and where and in what way messages will be handled
			var handlerService = new DefaultMessageHandlerService<PSOBBPatchPacketPayloadClient, SessionMessageContext<PSOBBPatchPacketPayloadServer>>();
			var dispatcher = new InPlaceNetworkMessageDispatchingStrategy<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>(handlerService);

			//Bind one of the default handlers
			handlerService.Bind<PSOBBPatchPacketPayloadClient>(new DefaultPatchMessageHandler(Logger));

			return new BoomaPatchManagedSession(options, context.Connection, context.Details, messageServices, dispatcher);
		}
	}
}