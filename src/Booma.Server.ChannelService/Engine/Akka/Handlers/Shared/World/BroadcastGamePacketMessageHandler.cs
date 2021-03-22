using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.Lobby
{
	[ActorMessageHandler(typeof(InstanceActor))]
	[ActorMessageHandler(typeof(LobbyActor))]
	public sealed class BroadcastGamePacketMessageHandler : BaseActorMessageHandler<BroadcastGamePacketMessage>
	{
		private IActorState<IActorMessageBroadcaster<LobbyActorGroupType>> LobbyMessageBroadcaster { get; }

		public BroadcastGamePacketMessageHandler(IActorState<IActorMessageBroadcaster<LobbyActorGroupType>> lobbyMessageBroadcaster)
		{
			LobbyMessageBroadcaster = lobbyMessageBroadcaster ?? throw new ArgumentNullException(nameof(lobbyMessageBroadcaster));
		}

		/// <inheritdoc />
		public override Task HandleMessageAsync(EntityActorMessageContext context, BroadcastGamePacketMessage message, CancellationToken token = new CancellationToken())
		{
			LobbyMessageBroadcaster.Data.BroadcastMessage(LobbyActorGroupType.Players, new SendGamePacketMessage(message.Packet));
			return Task.CompletedTask;
		}
	}
}
