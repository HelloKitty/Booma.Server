using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Booma
{
	public sealed class ExternalPlayerDataServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//CharacterDataEventPayloadFactory : ICharacterDataEventPayloadFactory
			builder.RegisterType<CharacterDataSnapshotFactory>()
				.As<ICharacterDataSnapshotFactory>()
				.InstancePerLifetimeScope();
		}
	}
}
