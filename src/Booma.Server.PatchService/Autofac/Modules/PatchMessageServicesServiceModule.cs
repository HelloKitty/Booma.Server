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

			builder.RegisterType<InPlaceNetworkMessageDispatchingStrategy<PSOBBPatchPacketPayloadClient, PSOBBPatchPacketPayloadServer>>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
		}
	}
}
