using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace Booma
{
	/// <summary>
	/// Service module for general game server services.
	/// </summary>
	public sealed class GameGeneralServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Used for building redirectors.
			builder.RegisterType<DefaultSessionRedirectorFactory>()
				.As<ISessionRedirectorFactory>()
				.SingleInstance();

			builder.RegisterType<MutableSessionTokenStorageRepository>()
				.AsImplementedInterfaces()
				.AsSelf()
				.InstancePerLifetimeScope();

			builder.RegisterType<GameEngineFrameworkManager>()
				.AsSelf()
				.InstancePerLifetimeScope();
		}
	}
}
