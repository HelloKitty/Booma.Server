using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Glader.Essentials;
using GladNet;
using MEAKKA;
using System.Reflection;

namespace Booma
{
	public sealed class MEAKKAServiceModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Create the root of the actor system.
			builder.Register(context =>
				{
					ActorSystem system = ActorSystem.Create("Root");
					return system;
				})
				.AsSelf()
				.As<ActorSystem>()
				.As<IActorRefFactory>()
				.AutoActivate()
				.OwnedByLifetimeScope()
				.SingleInstance();

			//MutableCharacterActorReferenceContainer : ICharacterActorReferenceContainer
			builder.RegisterType<MutableCharacterActorReferenceContainer>()
				.As<ICharacterActorReferenceContainer>()
				.OwnedByLifetimeScope()
				.InstancePerLifetimeScope();
		}
	}
}
