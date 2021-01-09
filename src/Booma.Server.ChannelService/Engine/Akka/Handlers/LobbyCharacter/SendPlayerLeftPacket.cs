using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;
using MEAKKA;

namespace Booma.LobbyCharacter
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	public sealed class SendPlayerLeftPacket : BaseActorMessageHandler<PlayerLeftWorldEventMessage>
	{
		private IActorState<IMessageSendService<PSOBBGamePacketPayloadServer>> SendService { get; }

		public SendPlayerLeftPacket(IActorState<IMessageSendService<PSOBBGamePacketPayloadServer>> sendService)
		{
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, PlayerLeftWorldEventMessage message, CancellationToken token = default)
		{
			await SendService.Data.SendMessageAsync(new BlockOtherPlayerLeaveLobbyEventPayload((byte) message.Slot, 0), token);
		}
	}
}
