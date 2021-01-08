using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Provides access to the character actor reference.
	/// </summary>
	public interface ICharacterActorReferenceContainer : IDisposable
	{
		/// <summary>
		/// Actor reference for the character.
		/// </summary>
		IActorRef Reference { get; set; }

		/// <summary>
		/// Indicates if the character actor is available.
		/// </summary>
		bool IsAvailable { get; }

		/// <summary>
		/// The entity guid associated with the character actor.
		/// </summary>
		NetworkEntityGuid EntityGuid { get; set; }
	}

	public sealed class MutableCharacterActorReferenceContainer : ICharacterActorReferenceContainer
	{
		/// <inheritdoc />
		public IActorRef Reference { get; set; }

		/// <inheritdoc />
		public bool IsAvailable => Reference != null && !Reference.IsNobody();

		/// <inheritdoc />
		public NetworkEntityGuid EntityGuid { get; set; } = NetworkEntityGuid.Empty;

		public void Dispose()
		{
			if (IsAvailable)
				Reference.TellSelf(new ActorOwnerDisposedMessage());
		}
	}
}
