﻿using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Autofac;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Service module for <see cref="InstanceActor"/>
	/// </summary>
	public sealed class InstanceActorServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule<EntityActorServiceModule<InstanceActor>>();

			//InMemoryCharacterLobbySlotRepository : ICharacterLobbySlotRepository
			builder.RegisterType<InMemoryCharacterInstanceSlotRepository>()
				.AsImplementedInterfaces()
				.InstancePerLifetimeScope();
		}
	}
}
