using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
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
		private IWelcomeMessageFactory WelcomeMessageFactory { get; }

		public BoomaGameManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, 
			SessionMessageBuildingServiceContext<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageServices, 
			INetworkMessageDispatchingStrategy<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageDispatcher,
			SessionMessageInterfaceServiceContext<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageInterfaces,
			[NotNull] IWelcomeMessageFactory welcomeMessageFactory)
			: base(networkOptions, connection, details, messageServices, messageDispatcher, messageInterfaces)
		{
			WelcomeMessageFactory = welcomeMessageFactory ?? throw new ArgumentNullException(nameof(welcomeMessageFactory));
		}

		/// <inheritdoc />
		protected override void OnSessionInitialized()
		{
			base.OnSessionInitialized();

			//Send the client the expected welcome message.
			SendService.SendMessageAsync(WelcomeMessageFactory.Create(EmptyFactoryContext.Instance));
		}
	}
}