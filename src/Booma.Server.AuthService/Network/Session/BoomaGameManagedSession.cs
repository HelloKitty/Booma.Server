using System;
using System.Collections.Generic;
using System.Linq;
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
	/// GladNet <see cref="BaseTcpManagedSession{TPayloadReadType,TPayloadWriteType}"/> Game Server Peer/Session implementation.
	/// </summary>
	public sealed class BoomaGameManagedSession : BaseBoomaManagedSession<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>
	{
		private INetworkCryptoInitializable CryptoInitializable { get; }

		private Random RandomGenerator { get; } = new Random();

		public BoomaGameManagedSession(NetworkConnectionOptions networkOptions, SocketConnection connection, SessionDetails details, 
			SessionMessageBuildingServiceContext<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageServices, 
			INetworkMessageDispatchingStrategy<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageDispatcher,
			INetworkMessageInterface<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer> messageInterface,
			[NotNull] INetworkCryptoInitializable cryptoInitializable)
			: base(networkOptions, connection, details, messageServices, messageDispatcher, messageInterface)
		{
			CryptoInitializable = cryptoInitializable ?? throw new ArgumentNullException(nameof(cryptoInitializable));
		}

		/// <inheritdoc />
		protected override void OnSessionInitialized()
		{
			base.OnSessionInitialized();

			string copyrightMessage = "Phantasy Star Online Blue Burst Game Server. Copyright 1999-2004 SONICTEAM.";

			//01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F 10 11 12 13 14 15 16 17 18 19 1A 1B 1C 1D 1E 1F 20 21 22 23 24 25 26 27 28 29 2A 2B 2C 2D 2E 2F 30
			//byte[] testIV = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0E, 0x0F, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20, 0x21, 0x22, 0x23, 0x24, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30 };
			
			byte[] serverIV = new byte[48];
			for (int i = 0; i < serverIV.Length; i++)
				serverIV[i] = (byte) (RandomGenerator.Next(0, 256) % 255);

			byte[] clientIV = new byte[48];
			for(int i = 0; i < clientIV.Length; i++)
				clientIV[i] = (byte) (RandomGenerator.Next(0, 256) % 255);

			CryptoInitializable.EncryptionInitializable.Initialize(serverIV.ToArray());
			CryptoInitializable.DecryptionInitializable.Initialize(clientIV.ToArray());

			//Instead of using the sendservice this actually sends the message DIRECTLY without any delay.
			SendService.SendMessageAsync(new SharedWelcomePayload(copyrightMessage, serverIV.ToArray(), clientIV.ToArray()));
		}
	}
}