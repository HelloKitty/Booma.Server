﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.World
{
	[ActorMessageHandler(typeof(InstanceActor))]
	[ActorMessageHandler(typeof(LobbyActor))]
	public sealed class BroadcastGamePacketMessageHandler : BaseActorMessageHandler<BroadcastGamePacketMessage>
	{
		private IActorState<IActorMessageBroadcaster<WorldActorGroupType>> LobbyMessageBroadcaster { get; }

		public BroadcastGamePacketMessageHandler(IActorState<IActorMessageBroadcaster<WorldActorGroupType>> lobbyMessageBroadcaster)
		{
			LobbyMessageBroadcaster = lobbyMessageBroadcaster ?? throw new ArgumentNullException(nameof(lobbyMessageBroadcaster));
		}

		/// <inheritdoc />
		public override Task HandleMessageAsync(EntityActorMessageContext context, BroadcastGamePacketMessage message, CancellationToken token = new CancellationToken())
		{
			LobbyMessageBroadcaster.Data.BroadcastMessage(WorldActorGroupType.Players, new SendGamePacketMessage(message.Packet));
			return Task.CompletedTask;
		}
	}
}
