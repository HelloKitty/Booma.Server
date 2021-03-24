﻿using System;
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
	/// Message handler for <see cref="BlockNetworkCommand62EventClientPayload"/>.
	/// Routes BlockNetworkCommand6D payloads into Akka through the client's <see cref="ICharacterActorReferenceContainer"/>.
	/// </summary>
	public sealed class BlockNetworkCommand62MessagedHandler : GameMessageHandler<BlockNetworkCommand62EventClientPayload>
	{
		/// <summary>
		/// Actor reference container.
		/// </summary>
		private ICharacterActorReferenceContainer ActorReference { get; }

		private ILog Logger { get; }

		public BlockNetworkCommand62MessagedHandler(ICharacterActorReferenceContainer actorReference, ILog logger)
		{
			ActorReference = actorReference ?? throw new ArgumentNullException(nameof(actorReference));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockNetworkCommand62EventClientPayload message, CancellationToken token = default)
		{
			//This is TEMP disabled until we have implemented Lobby -> Game or Game -> Lobby Actor creation
			/*if(!ActorReference.IsAvailable)
				await context.ConnectionService.DisconnectAsync();*/
			if (!ActorReference.IsAvailable && Logger.IsErrorEnabled)
				Logger.Error($"Unexpected DEADLETTER due to no valid actor reference. TODO: We need to make sure this never occurs and disconnect one day.");

			//TODO: Validate packet data!
			ActorReference.Reference.TellSelf(new BroadcastGamePacketMessage(new BlockNetworkCommand62EventServerPayload(message.Command), ActorReference.EntityGuid));
		}
	}
}
