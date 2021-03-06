﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Character
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	[ActorMessageHandler(typeof(InstanceCharacterActor))]
	public sealed class LeaveWorldOnActorOwnerDisposed : BaseActorMessageHandler<ActorOwnerDisposedMessage>
	{
		private IActorState<NetworkEntityGuid> GuidState { get; }

		public LeaveWorldOnActorOwnerDisposed(IActorState<NetworkEntityGuid> guidState)
		{
			GuidState = guidState ?? throw new ArgumentNullException(nameof(guidState));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, ActorOwnerDisposedMessage message, CancellationToken token = new CancellationToken())
		{
			//Owner/client/session has disposed of this player actor in some fashion
			//so we should tell the lobby (parent) that we are leaving
			context.ActorContext.Parent
				.TellEntity(new LeaveWorldRequestMessage(GuidState.Data));

			//Ack back the dispose request.
			message.Answer(context.Sender, true);

			//Stops us from handling ANY more messages.
			context.ActorContext.Stop(context.ActorContext.Self);
		}
	}
}
