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
					channelActor.Tell(new CreateLobbyMessage());

					return new EntityActorGenericAdapter<RootChannelActor>(channelActor);
				})
				.As<IEntityActorRef<RootChannelActor>>()
				.AutoActivate()
				.SingleInstance();
		}
	}
}
