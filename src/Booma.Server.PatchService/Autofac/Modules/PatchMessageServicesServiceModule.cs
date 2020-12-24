using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma.Proxy;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Patch service message services (dependencies involved in messages) service module.
	/// </summary>
	public sealed class PatchMessageServicesServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//This is just message serialization stuff. We don't need to make this instance per, it should be stateless.
			builder.RegisterType<SessionMessageBuildingServiceContext<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<InPlaceNetworkMessageDispatchingStrategy<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
		}
	}
}
