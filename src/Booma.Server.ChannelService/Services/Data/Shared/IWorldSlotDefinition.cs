using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using MEAKKA;

namespace Booma
{
	public interface IWorldSlotDefinition
	{
		/// <summary>
		/// Slot number.
		/// </summary>
		public int Slot { get; }

		/// <summary>
		/// The actor reference for the slot.
		/// </summary>
		public IActorRef Actor { get; }

		/// <summary>
		/// Indicates if the character is initialized.
		/// (don't send messages to an uninitialized character reference)
		/// </summary>
		bool IsInitialized { get; set; }
	}
}
