using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;
using GladNet;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Message handler for <see cref="BlockNetworkCommand60EventClientPayload"/>.
	/// Routes Subcommand60 payloads into Akka through the client's <see cref="ICharacterActorReferenceContainer"/>.
	/// </summary>
	public sealed class Subcommand60MessageHandler : GameMessageHandler<BlockNetworkCommand60EventClientPayload>
	{
		/// <summary>
		/// Actor reference container.
		/// </summary>
		private ICharacterActorReferenceContainer ActorReference { get; }

		private ILog Logger { get; }

		public Subcommand60MessageHandler(ICharacterActorReferenceContainer actorReference, ILog logger)
		{
			ActorReference = actorReference ?? throw new ArgumentNullException(nameof(actorReference));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockNetworkCommand60EventClientPayload message, CancellationToken token = new CancellationToken())
		{
			//This is TEMP disabled until we have implemented Lobby -> Game or Game -> Lobby Actor creation
			/*if(!ActorReference.IsAvailable)
				await context.ConnectionService.DisconnectAsync();*/
			if (!ActorReference.IsAvailable && Logger.IsErrorEnabled)
				Logger.Error($"Unexpected DEADLETTER due to no valid actor reference. TODO: We need to make sure this never occurs and disconnect one day.");

			//TODO: Validate packet data!
			ActorReference.Reference.TellSelf(new BroadcastGamePacketMessage(new BlockNetworkCommand60EventServerPayload(message.Command), ActorReference.EntityGuid));
		}
	}
}
