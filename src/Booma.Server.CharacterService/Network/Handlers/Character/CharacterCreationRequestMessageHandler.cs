using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Message handler that handles character creation requests <see cref="CharacterCreationRequestPayload"/>
	/// and also dressing room stuff but that is not yet implemented.
	/// Sends a response of: <see cref="CharacterCharacterSelectionAckPayload"/>
	/// </summary>
	public sealed class CharacterCreationRequestMessageHandler : GameRequestMessageHandler<CharacterCreationRequestPayload, CharacterCharacterSelectionAckPayload>
	{
		/// <summary>
		/// The character data repository.
		/// </summary>
		private ICharacterDataRepository CharacterRepository { get; }

		public CharacterCreationRequestMessageHandler(ICharacterDataRepository characterRepository)
		{
			CharacterRepository = characterRepository ?? throw new ArgumentNullException(nameof(characterRepository));
		}

		/// <inheritdoc />
		protected override async Task<CharacterCharacterSelectionAckPayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterCreationRequestPayload message, CancellationToken token = default)
		{
			//TODO: Can we send PSOBB anything to say creation failed??
			bool result = await CharacterRepository.CreateAsync(message.CharacterData, token);
			return new CharacterCharacterSelectionAckPayload(message.SlotSelected, CharacterSelectionAckType.BB_CHAR_ACK_UPDATE);
		}
	}
}
