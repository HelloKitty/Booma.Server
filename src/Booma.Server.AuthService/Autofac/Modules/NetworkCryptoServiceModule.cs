using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma.Proxy;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Service module for network cryptographic operations.
	/// Registers the following services:
	/// <see cref="INetworkCryptoService"/> and <see cref="INetworkCryptoInitializable"/>
	/// and a custom <see cref="INetworkMessageInterface{TPayloadReadType,TPayloadWriteType}"/> that supports
	/// encryption/decryption.
	/// </summary>
	public sealed class NetworkCryptoServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//We need instance per lifetime scope, it's important each session gets unique crypto service.
			//BlowFishNetworkCryptoService : INetworkCryptoService, INetworkCryptoInitializable
			builder.RegisterType<BlowFishNetworkCryptoService>()
				.As<INetworkCryptoService>()
				.As<INetworkCryptoInitializable>()
				.AsSelf()
				.InstancePerLifetimeScope()
				.OwnedByLifetimeScope();

			//Another case of needing a unique instance per lifetimescope.
			builder.RegisterType<EncryptedNetworkMessageInterface<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>>()
				.As<INetworkMessageInterface<PSOBBGamePacketPayloadClient, PSOBBGamePacketPayloadServer>>()
				.AsSelf()
				.InstancePerLifetimeScope()
				.OwnedByLifetimeScope();
		}
	}
}
