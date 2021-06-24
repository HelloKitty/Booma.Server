using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GladNet;

namespace Booma.Server
{
	/// <summary>
	/// Chat message handler. Client sends: <see cref="BlockTextChatMessageRequestPayload"/>
	/// </summary>
	public sealed class BlockTextChatMessageRequestMessageHandler : GameMessageHandler<BlockTextChatMessageRequestPayload>
	{
		private GGDBFInitializer DataInitializer { get; }

		public BlockTextChatMessageRequestMessageHandler(GGDBFInitializer dataInitializer)
		{
			DataInitializer = dataInitializer ?? throw new ArgumentNullException(nameof(dataInitializer));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockTextChatMessageRequestPayload message, CancellationToken token = default)
		{
			if (message.ChatMessage.ToUpper() == "/GGDBF RELOAD")
			{
				await DataInitializer.InitializeAsync(token);
				await context.MessageService.SendMessageAsync(new SharedCreateMessageBoxEventPayload("GGDBF system reloaded."), token);
			}
		}
	}
}
