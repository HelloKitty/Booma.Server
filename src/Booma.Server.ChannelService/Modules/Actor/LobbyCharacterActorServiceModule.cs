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
	/// Service module for <see cref="LobbyCharacterActor"/>
	/// </summary>
	public sealed class LobbyCharacterActorServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule<EntityActorServiceModule<LobbyCharacterActor>>();
		}
	}
}
