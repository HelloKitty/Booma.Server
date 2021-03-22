using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(LobbyCharacterActor))]
	[ActorMessageHandler(typeof(InstanceCharacterActor))]
	public sealed class PacketSendMessageHandler : BaseActorMessageHandler<SendGamePacketMessage>
	{
		private IActorState<IMessageSendService<PSOBBGamePacketPayloadServer>> SendService { get; }

		public PacketSendMessageHandler(IActorState<IMessageSendService<PSOBBGamePacketPayloadServer>> sendService)
		{
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, SendGamePacketMessage message, CancellationToken token = default)
		{
			await SendService.Data.SendMessageAsync(message.Packet, token);
		}
	}
}
