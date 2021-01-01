using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Autofac;
using Booma;
using Common.Logging;
using GladNet;
using JetBrains.Annotations;

namespace Booma
{
	/// <summary>
	/// GladNet <see cref="BaseBoomaServerApplication{TManagedSessionType}"/> implementation for PSO Booma Patch Service.
	/// </summary>
	public sealed class BoomaPatchServerApplication : BaseBoomaServerApplication<BoomaPatchManagedSession>
	{
		public BoomaPatchServerApplication(NetworkAddressInfo serverAddress, ILog logger) 
			: base(serverAddress, logger)
		{

		}

		/// <inheritdoc />
		protected override ContainerBuilder RegisterServices(ContainerBuilder builder)
		{
			//Patch service modules
			builder.RegisterModule<DefaultLoggingServiceModule>()
				.RegisterModule<PatchSerializationServiceModule>()
				.RegisterModule<PatchMessageHandlerServiceModule>()
				.RegisterModule<ServerSessionServiceModule<BoomaPatchManagedSession>>();

			//We want in-place handling on the patch server since really message reading throughput isn't required
			//and this handling strategy avoids greedy clients from saturating the CPU with handling packets.
			//1 packet is read and handled at a time with this strategy.
			builder.RegisterModule<InPlaceMessageDispatchingServiceModule<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>>();

			builder
				.RegisterInstance(BuildNetworkOptions())
				.AsSelf()
				.SingleInstance();

			return builder;
		}

		private static NetworkConnectionOptions BuildNetworkOptions()
		{
			return new NetworkConnectionOptions(NetworkPatchPacketConstants.PATCH_PACKET_HEADER_SIZE, NetworkPatchPacketConstants.PATCH_PACKET_HEADER_SIZE, NetworkPatchPacketConstants.PATCH_PACKET_MAXIMUM_SIZE);
		}

		/// <inheritdoc />
		protected override bool IsClientAcceptable(Socket connection)
		{
			return true;
		}
	}
}