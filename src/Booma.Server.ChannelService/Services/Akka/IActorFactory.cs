using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class ActorCreationContext
	{
		/// <summary>
		/// The factory responsible for creating the actor
		/// reference.
		/// (Using this instead of global reference factory creates child/parent tree relationships.)
		/// </summary>
		public IActorRefFactory ActorReferenceFactory { get; }

		public ActorCreationContext(IActorRefFactory actorReferenceFactory)
		{
			ActorReferenceFactory = actorReferenceFactory ?? throw new ArgumentNullException(nameof(actorReferenceFactory));
		}
	}

	/// <summary>
	/// Contract for types that can create actors of <typeparamref name="TActorType"/>.
	/// Returning <see cref="IActorRef"/> from creation context <see cref="ActorCreationContext"/>.
	/// </summary>
	/// <typeparam name="TActorType">The actor type.</typeparam>
	public interface IActorFactory<out TActorType> : IFactoryCreatable<IActorRef, ActorCreationContext>
		where TActorType : IEntityActor
	{

	}

	public sealed class DefaultFactorFactory<TActorType> : IActorFactory<TActorType> 
		where TActorType : ActorBase, IEntityActor
	{
		/// <summary>
		/// Dependency resolver for the actor creation.
		/// </summary>
		private IDependencyResolver ActorResolver { get; }

		public DefaultFactorFactory(ILifetimeScope container, ActorSystem system)
		{
			if (container == null) throw new ArgumentNullException(nameof(container));
			if (system == null) throw new ArgumentNullException(nameof(system));

			//TODO: We should invert control of the dependency resolver implementation
			ActorResolver = new AutoFacDependencyResolver(container, system);
		}

		/// <inheritdoc />
		public IActorRef Create(ActorCreationContext context)
		{
			IActorRef actorRef = context.ActorReferenceFactory.ActorOf(ActorResolver.Create<TActorType>(), typeof(TActorType).Name);

			if (actorRef.IsNobody())
				throw new InvalidOperationException($"Failed to create Actor: {typeof(TActorType).Name}. Path: {actorRef.Path}");

			return actorRef;
		}
	}
}
