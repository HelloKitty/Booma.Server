using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
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

		private ICharacterDataRepository CharacterDataRepository { get; }

		public CharacterCharacterSelectionRequestMessageHandler(ILog logger,
			ICharacterDataRepository characterDataRepository)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			CharacterDataRepository = characterDataRepository ?? throw new ArgumentNullException(nameof(characterDataRepository));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterCharacterSelectionRequestPayload message, CancellationToken token = new CancellationToken())
		{
			switch (message.SelectionType)
			{
				case CharacterSelectionType.Preview:
					await HandlePreviewCharacterAsync(context, message.SlotSelected, token);
					break;
				case CharacterSelectionType.PlaySelection:
					await HandleSelectCharacterAsync(context, message.SlotSelected, token);
					break;
				default:
					await HandleInvalidSelectionTypeAsync(context, message.SelectionType);
					break;
			}
		}

		private async Task HandleSelectCharacterAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, int slot, CancellationToken token = default)
		{
			//This is kinda dumb but we just assume the client isn't lying.
			//Some other point before entering a block they'll be caught
			//So no reason to mitigate anything spoofed/hacked here
			await context.MessageService.SendMessageAsync(new CharacterCharacterSelectionAckPayload(slot, CharacterSelectionAckType.BB_CHAR_ACK_SELECT), token);
		}

		private async Task HandlePreviewCharacterAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, int slot, CancellationToken token = default)
		{
			//Otherwise we should send the character.
			if (!await CharacterDataRepository.ContainsAsync(slot, token))
				await context.MessageService.SendMessageAsync(new CharacterCharacterSelectionAckPayload(slot, CharacterSelectionAckType.BB_CHAR_ACK_NONEXISTANT), token);
			else
			{
				PlayerCharacterDataModel data = await CharacterDataRepository.RetrieveAsync(slot, token);
				await context.MessageService.SendMessageAsync(new CharacterCharacterUpdateResponsePayload((byte)slot, data), token);
			}
		}

		private async Task HandleInvalidSelectionTypeAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterSelectionType messageSelectionType)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (Logger.IsErrorEnabled)
				Logger.Error($"Encountered unknown {nameof(CharacterSelectionType)} in Message: {nameof(CharacterCharacterSelectionRequestPayload)}. Value: {messageSelectionType}");

			//We disconnect because we cannot handle this
			await context.ConnectionService.DisconnectAsync();
		}
	}
}
