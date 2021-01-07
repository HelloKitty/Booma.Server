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
					IActorRef channelActor = context
						.Resolve<IActorFactory<RootChannelActor>>()
						.Create(new ActorCreationContext(context.Resolve<IActorRefFactory>()));

					channelActor.Tell(new EntityActorStateInitializeMessage<EmptyFactoryContext>(EmptyFactoryContext.Instance));

					//TODO: We should make this configurable
					for(int i = 0; i < 15; i++)
						channelActor.Tell(new CreateLobbyMessage(i));

					return new EntityActorGenericAdapter<RootChannelActor>(channelActor);
				})
				.As<IEntityActorRef<RootChannelActor>>()
				.AutoActivate()
				.SingleInstance();

			//Lobby registry
			builder.RegisterType<InMemoryLobbyEntryRepository>()
				.As<ILobbyEntryRepository>()
				.InstancePerLifetimeScope();
		}
	}
}
