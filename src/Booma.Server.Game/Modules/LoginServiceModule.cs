using System;
using System.Collections.Generic;
using System.Text;
using Autofac;
using Booma;
using Booma;
using Glader.ASP.Authentication;
using Glader.Essentials;

namespace Booma
{
	//This exists for services that don't want any special event to occur
	//when login responses happen.
	/// <summary>
	/// Service module for all the services required to properly handle
	/// <see cref="SharedLoginRequest93Payload"/> and <see cref="LoginRequestMessageHandler"/>
	/// </summary>
	public class LoginServiceModule : Module
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

	/// <summary>
	/// Service module for all the services required to properly handle
	/// <see cref="SharedLoginRequest93Payload"/> and <see cref="LoginRequestMessageHandler"/>
	/// <typeparam name="TLoginResponseHandlingStrategyType">Represents the handling strategy for login responses.</typeparam>
	/// </summary>
	public sealed class LoginServiceModule<TLoginResponseHandlingStrategyType> : LoginServiceModule
		where TLoginResponseHandlingStrategyType : LoginResponseSentEventListener
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//This is the most critical handler for login response.
			//It should start-off the whole session or redirect it or something.
			//Each service may have a different explicit next step after login/auth.
			builder.RegisterType<TLoginResponseHandlingStrategyType>()
				.AsSelf()
				.As<IGameInitializable>()
				.InstancePerLifetimeScope();
		}
	}
}
