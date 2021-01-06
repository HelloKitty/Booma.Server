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

			builder.Register(context =>
				{
					// Create the dependency resolver
					IDependencyResolver resolver = new AutoFacDependencyResolver(context.Resolve<ILifetimeScope>(), context.Resolve<ActorSystem>());
					return resolver;
				})
				.As<IDependencyResolver>()
				.AutoActivate()
				.OwnedByLifetimeScope()
				.SingleInstance();

			RegisterActorType<RootChannelActor>(builder);

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
				.As<IEntityActor<RootChannelActor>>()
				.AutoActivate()
				.SingleInstance();

			//Must be instance per lifetime scope since it depends
			//on the lifetimescope for resolving actor dependencies.
			builder.RegisterGeneric(typeof(DefaultFactorFactory<>))
				.As(typeof(IActorFactory<>))
				.InstancePerLifetimeScope();
		}

		private void RegisterActorType<TActorType>(ContainerBuilder builder)
			where TActorType : IEntityActor
		{
			builder.RegisterType<TActorType>()
				.AsSelf();

			builder.Register(context =>
				{
					var handlers = context.ResolveNamed<IEnumerable<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>>(typeof(TActorType).Name);
					return new ActorMessageHandlerService<TActorType>(handlers);
				})
				.As<ActorMessageHandlerService<TActorType>>()
				.InstancePerLifetimeScope();

			foreach (var handler in GetType()
				.Assembly
				.GetTypes()
				.Where(t => t.IsAssignableTo<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>())
				.Where(t => t.GetCustomAttribute<ActorMessageHandlerAttribute>()?.ActorType == typeof(TActorType)))
			{
				//No longer sharing handlers anymore.
				builder.RegisterType(handler)
					.Named<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>(typeof(TActorType).Name)
					.InstancePerLifetimeScope();
			}
		}
	}
}
