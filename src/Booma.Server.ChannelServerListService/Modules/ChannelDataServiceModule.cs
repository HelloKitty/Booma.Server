using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Booma
{
	public sealed class ChannelDataServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterType<ChannelListNetworkedMenu>()
				.AsSelf()
				.InstancePerLifetimeScope();
		}
	}
}
