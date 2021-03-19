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
	/// Service module for <see cref="LobbyActor"/>
	/// </summary>
	public sealed class LobbyActorServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule<EntityActorServiceModule<LobbyActor>>();

			//InMemoryCharacterLobbySlotRepository : ICharacterLobbySlotRepository
			builder.RegisterType<InMemoryCharacterLobbySlotRepository>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
		}
	}
}
