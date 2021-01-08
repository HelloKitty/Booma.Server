﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using GladNet;
using MEAKKA;

namespace Booma
{
	public sealed class Subcommand60EchoMessageHandler : GameMessageHandler<BlockNetworkCommand60EventClientPayload>
	{
		private ICharacterActorReferenceContainer ActorReference { get; }

		public Subcommand60EchoMessageHandler(ICharacterActorReferenceContainer actorReference) 
		{
			ActorReference = actorReference ?? throw new ArgumentNullException(nameof(actorReference));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockNetworkCommand60EventClientPayload message, CancellationToken token = new CancellationToken())
		{
			if(!ActorReference.IsAvailable)
				await context.ConnectionService.DisconnectAsync();

			//TODO: Validate packet data!
			ActorReference.Reference.TellSelf(new BroadcastGamePacketMessage(new BlockNetworkCommand60EventServerPayload(message.Command), ActorReference.EntityGuid));
		}
	}
}
