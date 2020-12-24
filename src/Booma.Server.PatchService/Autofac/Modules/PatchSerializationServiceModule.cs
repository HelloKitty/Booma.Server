using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Booma
{
	/// <summary>
	/// Patch service packet/header serialization service module.
	/// </summary>
	public sealed class PatchSerializationServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//All the patch serializer stuff is stateless and can be shared
			//so we're going to SingleInstance it.
			builder
				.RegisterType<PatchPacketSerializer>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<PatchPacketHeaderFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();

			builder.RegisterType<PatchPacketHeaderSerializer>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
