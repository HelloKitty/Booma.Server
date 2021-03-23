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
	[ActorMessageHandler(typeof(LobbyActor))]
	public sealed class TryCreateCharacterActorInLobbyHandler : BaseActorMessageHandler<TryCreateCharacterRequestMessage>
	{
		private ICharacterLobbySlotRepository CharacterSlotRepository { get; }

		private IActorFactory<LobbyCharacterActor> LobbyCharacterFactory { get; }

		private ILog Logger { get; }

		private IActorState<NetworkEntityGuid> LobbyIdentifier { get; }

		private IActorState<IActorMessageBroadcaster<WorldActorGroupType>> LobbyMessageBroadcaster { get; }

		public TryCreateCharacterActorInLobbyHandler(ICharacterLobbySlotRepository characterSlotRepository, 
			IActorFactory<LobbyCharacterActor> lobbyCharacterFactory, 
			ILog logger, 
			IActorState<NetworkEntityGuid> lobbyIdentifier, 
			IActorState<IActorMessageBroadcaster<WorldActorGroupType>> lobbyMessageBroadcaster)
		{
			CharacterSlotRepository = characterSlotRepository ?? throw new ArgumentNullException(nameof(characterSlotRepository));
			LobbyCharacterFactory = lobbyCharacterFactory ?? throw new ArgumentNullException(nameof(lobbyCharacterFactory));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			LobbyIdentifier = lobbyIdentifier ?? throw new ArgumentNullException(nameof(lobbyIdentifier));
			LobbyMessageBroadcaster = lobbyMessageBroadcaster ?? throw new ArgumentNullException(nameof(lobbyMessageBroadcaster));
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

			IEntityActorRef<LobbyCharacterActor> lobbyCharacterActor = null;
			try
			{
				lobbyCharacterActor = LobbyCharacterFactory
					.Create(new ActorCreationContext(context.ActorContext));

				await CharacterSlotRepository
					.TryCreateAsync(new CharacterLobbySlot(message.CharacterData, slot, lobbyCharacterActor), token);

				if (Logger.IsInfoEnabled)
					Logger.Info($"Reserved Slot in Lobby: {LobbyIdentifier.Data.Identifier} Character Slot: {slot} Name: {message.CharacterData.Name}");

				LobbyMessageBroadcaster.Data.AddToGroup(WorldActorGroupType.Players, lobbyCharacterActor.Actor);

				//Send the actor path if we successfully slotted the character.
				message.AnswerSuccess(context.Sender, lobbyCharacterActor.Actor.Path.ToString());
			}
			catch (Exception e)
			{
				//Kill the actor, something went wrong.
				if (lobbyCharacterActor != null && !lobbyCharacterActor.Actor.IsNobody())
					lobbyCharacterActor.Actor.Tell(PoisonPill.Instance);

				if (Logger.IsErrorEnabled)
					Logger.Error($"Failed to create Lobby Character in Lobby: {LobbyIdentifier.Data.Identifier} for Character: {message.CharacterData.Name} Slot: {slot}. Reason: {e}");
			}
		}
	}
}
