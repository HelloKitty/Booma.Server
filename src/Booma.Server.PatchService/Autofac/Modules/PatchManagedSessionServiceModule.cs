using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Booma
{
	/// <summary>
	/// Patch service managed session service module.
	/// Registers <see cref="BoomaPatchManagedSession"/> and related services.
	/// </summary>
	public sealed class PatchManagedSessionServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Obviously we need a FRESH session per lifetime.
			builder.RegisterType<BoomaPatchManagedSession>()
				.InstancePerLifetimeScope();

			//Should be stateless, so we should be alright.
			builder.RegisterType<PatchManagedSessionFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
