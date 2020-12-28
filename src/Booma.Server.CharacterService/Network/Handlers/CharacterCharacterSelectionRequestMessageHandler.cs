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

		private async Task HandlePreviewCharacterAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, int slot)
		{
			//TODO: This is a demo/test character
			if (slot == 0)
			{
				await context.MessageService.SendMessageAsync(new CharacterCharacterUpdateResponsePayload((byte)slot, BuildDefaultCharacterData()));
				return;
			}
			
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

		private PlayerCharacterDataModel BuildDefaultCharacterData()
		{
			return new PlayerCharacterDataModel(new CharacterProgress(0, 1), String.Empty, new CharacterSpecialCustomInfo(0, CharacterModelType.Regular, 0), SectionId.Redria, CharacterClass.HUmar, new CharacterVersionData(0, 0, 0), new CharacterCustomizationInfo(0, 0, 0, 0, 0, new Vector3<ushort>(0, 0, 0), new Vector2<float>(0, 0)), "Glader", 0);
		}
	}
}
