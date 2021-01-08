using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma.Lobby
{
	[ActorMessageHandler(typeof(GameLobbyActor))]
	public sealed class TryCreateCharacterActorInLobbyHandler : BaseActorMessageHandler<TryCreateCharacterRequestMessage>
	{
		private ICharacterLobbySlotRepository CharacterSlotRepository { get; }

		private IActorFactory<LobbyCharacterActor> LobbyCharacterFactory { get; }

		private ILog Logger { get; }

		public TryCreateCharacterActorInLobbyHandler(ICharacterLobbySlotRepository characterSlotRepository, 
			IActorFactory<LobbyCharacterActor> lobbyCharacterFactory, 
			ILog logger)
		{
			CharacterSlotRepository = characterSlotRepository ?? throw new ArgumentNullException(nameof(characterSlotRepository));
			LobbyCharacterFactory = lobbyCharacterFactory ?? throw new ArgumentNullException(nameof(lobbyCharacterFactory));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, TryCreateCharacterRequestMessage message, CancellationToken token = default)
		{
			//TODO: Do more than just say we're full
			if (!await CharacterSlotRepository.HasAvailableSlotAsync(token))
			{
				message.AnswerFailure(context.Sender, CharacterActorCreationResponseCode.UnavailableSpace);
				return;
			}

			int slot = await CharacterSlotRepository
				.FirstAvailableSlotAsync(token);

			IActorRef lobbyCharacterActor = null;
			try
			{
				lobbyCharacterActor = LobbyCharacterFactory
					.Create(new ActorCreationContext(context.ActorContext));

				await CharacterSlotRepository
					.TryCreateAsync(new CharacterLobbySlot(message.CharacterData, slot, lobbyCharacterActor), token);

				if (Logger.IsInfoEnabled)
					Logger.Info($"Created Lobby Character Slot: {slot} Name: {message.CharacterData.Name}");

				//Send the actor path if we successfully slotted the character.
				message.AnswerSuccess(context.Sender, lobbyCharacterActor.Path.ToString());
			}
			catch (Exception e)
			{
				//Kill the actor, something went wrong.
				if (lobbyCharacterActor != null && !lobbyCharacterActor.IsNobody())
					lobbyCharacterActor.Tell(PoisonPill.Instance);

				if (Logger.IsErrorEnabled)
					Logger.Error($"Failed to create Lobby Character for Character: {message.CharacterData.Name} Slot: {slot}");
			}
		}
	}
}
