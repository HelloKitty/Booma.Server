using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Akka.Actor;
using Akka.Util;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Type-safe <see cref="IActorRef"/> references to a specified
	/// Entity Actor Type <typeparamref name="TActorType"/>.
	/// </summary>
	/// <typeparam name="TActorType"></typeparam>
	public interface IEntityActor<TActorType> : IActorRef
		where TActorType : IEntityActor
	{

	}

	public sealed class EntityActorGenericAdapter<TActorType> : IEntityActor<TActorType>, IActorRef 
		where TActorType : IEntityActor
	{
		private IActorRef Actor { get; }

		public EntityActorGenericAdapter(IActorRef actor)
		{
			Actor = actor ?? throw new ArgumentNullException(nameof(actor));
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Tell(object message, IActorRef sender)
		{
			Actor.Tell(message, sender);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(IActorRef other)
		{
			return Actor.Equals(other);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(IActorRef other)
		{
			return Actor.CompareTo(other);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISurrogate ToSurrogate(ActorSystem system)
		{
			return Actor.ToSurrogate(system);
		}

		//TODO: Examine nullable
		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(object? obj)
		{
			return Actor.CompareTo(obj);
		}

		/// <inheritdoc />
		public ActorPath Path
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Actor.Path;
		}
	}
}
