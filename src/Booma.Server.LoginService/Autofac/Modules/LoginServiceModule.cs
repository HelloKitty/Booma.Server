using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma;
using Booma.Proxy;
using GladMMO;

namespace Booma
{
	/// <summary>
	/// Service module for all the services required to properly handle
	/// <see cref="SharedLoginRequest93Payload"/> and <see cref="LoginRequestMessageHandler"/>
	/// </summary>
	public sealed class LoginServiceModule : Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Registers the auth service.
			builder.RegisterModule(new ServiceDiscoverableServiceModule<IAuthenticationService>(BoomaServiceType.AuthService));

			builder.RegisterType<SuccessfulLogin93ResponseMessageFactory>()
				.AsImplementedInterfaces()
				.SingleInstance();
		}
	}
}
