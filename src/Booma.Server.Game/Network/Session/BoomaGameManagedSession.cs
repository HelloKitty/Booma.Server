using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using FreecraftCore.Serializer;
using Glader.Essentials;
using GladNet;
using Pipelines.Sockets.Unofficial;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="BaseTcpManagedSession{TPayloadReadType,TPayloadWriteType}"/> Game Server Peer/Session implementation.
	/// </summary>
	public sealed class BoomaGameManagedSession : BaseBoomaManagedSession<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>
	{
		private GameEngineFrameworkManager Engine { get; }

		public BoomaGameManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, 
			SessionMessageBuildingServiceContext<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageServices, 
			INetworkMessageDispatchingStrategy<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageDispatcher,
			SessionMessageInterfaceServiceContext<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageInterfaces,
			GameEngineFrameworkManager engine)
			: base(networkOptions, connection, details, messageServices, messageDispatcher, messageInterfaces)
		{
			Engine = engine ?? throw new ArgumentNullException(nameof(engine));
		}

		/// <inheritdoc />
		protected override void OnSessionInitialized()
		{
			base.OnSessionInitialized();

			Task.Run(async () =>
			{
				await Engine.OnGameInitialized();
			});
		}

		public override async Task OnNetworkMessageReceivedAsync(NetworkIncomingMessage<PSOBBGamePacketPayloadClient> message, CancellationToken token = default)
		{
			//Must wait for the engine to be initialized before we can handle any incoming messages.
			await Engine.Initialized;
			await base.OnNetworkMessageReceivedAsync(message, token);
		}
	}
}