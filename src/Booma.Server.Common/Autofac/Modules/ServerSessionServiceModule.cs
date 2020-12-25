using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Base module for session types.
	/// Registers the following services:
	/// <para />
	/// <typeparamref name="TManagedSessionType"/>
	/// <para />
	/// <see cref="ManagedSessionFactory{TSessionType}"/> for <typeparamref name="TManagedSessionType"/>
	/// </summary>
	/// <typeparam name="TManagedSessionType"></typeparam>
	public class ServerSessionServiceModule<TManagedSessionType> : Module
		where TManagedSessionType : IManagedSession, IDisposableAttachable
	{
		protected sealed override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Obviously we need a FRESH session per lifetime.
			builder.RegisterType<TManagedSessionType>()
				.AsSelf()
				.InstancePerLifetimeScope();

			//Should be stateless, so we should be alright.
			builder.RegisterType<ManagedSessionFactory<TManagedSessionType>>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
