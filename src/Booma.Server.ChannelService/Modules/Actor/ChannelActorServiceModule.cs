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
	/// Service module for <see cref="RootChannelActor"/>
	/// </summary>
	public sealed class ChannelActorServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule<EntityActorServiceModule<RootChannelActor>>();

			//Create the root channel actor.
			builder.Register(context =>
				{
					IEntityActorRef<RootChannelActor> channelActor = context
						.Resolve<IActorFactory<RootChannelActor>>()
						.Create(new ActorCreationContext(context.Resolve<IActorRefFactory>()));

					channelActor.Actor.Tell(new EntityActorStateInitializeMessage<EmptyFactoryContext>(EmptyFactoryContext.Instance));

					//TODO: We should make this configurable
					for(int i = 0; i < 15; i++)
						channelActor.Actor.Tell(new CreateLobbyMessage(i));

					return channelActor;
				})
				.As<IEntityActorRef<RootChannelActor>>()
				.AutoActivate()
				.SingleInstance();

			//Lobby registry
			builder.RegisterType<InMemoryLobbyEntryRepository>()
				.As<ILobbyEntryRepository>()
				.InstancePerLifetimeScope();

			//Instance registry
			builder.RegisterType<InMemoryInstanceEntryRepository>()
				.As<IInstanceEntryRepository>()
				.InstancePerLifetimeScope();
		}
	}
}
