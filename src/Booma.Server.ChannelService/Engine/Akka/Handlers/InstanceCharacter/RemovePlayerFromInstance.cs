using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MEAKKA;

namespace Booma.InstanceCharacter
{
	[ActorMessageHandler(typeof(InstanceActor))]
	public sealed class RemovePlayerFromInstance : BaseActorMessageHandler<LeaveWorldRequestMessage>
	{
		private IActorState<IActorMessageBroadcaster<WorldActorGroupType>> MessageBroadcaster { get; }

		public RemovePlayerFromInstance(IActorState<IActorMessageBroadcaster<WorldActorGroupType>> messageBroadcaster)
		{
			MessageBroadcaster = messageBroadcaster ?? throw new ArgumentNullException(nameof(messageBroadcaster));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, LeaveWorldRequestMessage message, CancellationToken token = new CancellationToken())
		{
			MessageBroadcaster.Data.RemoveFromGroup(WorldActorGroupType.Players, context.Sender);
		}
	}
}
