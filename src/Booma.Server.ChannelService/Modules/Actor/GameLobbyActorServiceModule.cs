using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Autofac;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Service module for <see cref="GameLobbyActor"/>
	/// </summary>
	public sealed class GameLobbyActorServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule<EntityActorServiceModule<GameLobbyActor>>();

			//InMemoryCharacterLobbySlotRepository : ICharacterLobbySlotRepository
			builder.RegisterType<InMemoryCharacterLobbySlotRepository>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
		}
	}
}
