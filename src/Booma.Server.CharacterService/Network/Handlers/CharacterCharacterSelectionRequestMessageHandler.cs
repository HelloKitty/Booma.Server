using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// <see cref="CharacterCharacterSelectionRequestPayload"/> message handler.
	/// Character screen sends this to select or preview information about the character.
	/// </summary>
	public sealed class CharacterCharacterSelectionRequestMessageHandler : GameMessageHandler<CharacterCharacterSelectionRequestPayload>
	{
		private ILog Logger { get; }

		public CharacterCharacterSelectionRequestMessageHandler([JetBrains.Annotations.NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterCharacterSelectionRequestPayload message, CancellationToken token = new CancellationToken())
		{
			switch (message.SelectionType)
			{
				case CharacterSelectionType.Preview:
					await HandlePreviewCharacterAsync(context, message.SlotSelected);
					break;
				case CharacterSelectionType.PlaySelection:
					break;
				default:
					await HandleInvalidSelectionTypeAsync(context, message.SelectionType);
					break;
			}
		}

		private async Task HandlePreviewCharacterAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, int slot)
		{
			//Client expects 0xE5 to be sent. This is CharacterCharacterUpdateResponsePayload.
			await context.MessageService.SendMessageAsync(new CharacterCharacterSelectionAckPayload(slot, CharacterSelectionAckType.BB_CHAR_ACK_NONEXISTANT));
		}

		private async Task HandleInvalidSelectionTypeAsync([NotNull] SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterSelectionType messageSelectionType)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (Logger.IsErrorEnabled)
				Logger.Error($"Encountered unknown {nameof(CharacterSelectionType)} in Message: {nameof(CharacterCharacterSelectionRequestPayload)}. Value: {messageSelectionType}");

			//We disconnect because we cannot handle this
			await context.ConnectionService.DisconnectAsync();
		}
	}
}
