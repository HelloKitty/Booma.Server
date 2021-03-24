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
	public sealed class BlockFinishedGameBurstingRequestMessageHandler : GameMessageHandler<BlockFinishedGameBurstingRequestPayload>
	{
		/// <summary>
		/// Actor reference container.
		/// </summary>
		private ICharacterActorReferenceContainer ActorReference { get; }

		private ILog Logger { get; }

		public BlockFinishedGameBurstingRequestMessageHandler(ICharacterActorReferenceContainer actorReference, ILog logger)
		{
			ActorReference = actorReference ?? throw new ArgumentNullException(nameof(actorReference));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockFinishedGameBurstingRequestPayload message, CancellationToken token = default)
		{
			//This is TEMP disabled until we have implemented Lobby -> Game or Game -> Lobby Actor creation
			/*if(!ActorReference.IsAvailable)
				await context.ConnectionService.DisconnectAsync();*/
			if (!ActorReference.IsAvailable && Logger.IsErrorEnabled)
				Logger.Error($"Unexpected DEADLETTER due to no valid actor reference. TODO: We need to make sure this never occurs and disconnect one day.");

			//TODO: Validate packet data!
			ActorReference.Reference.TellSelf(new BroadcastGamePacketMessage(new BlockNetworkCommand60EventServerPayload(new Sub60GameBurstingCompleteEventCommand()), ActorReference.EntityGuid));
		}
	}
}
