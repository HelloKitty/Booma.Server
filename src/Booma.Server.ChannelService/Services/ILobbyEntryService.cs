using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;
using Glader.Essentials;
using GladNet;
using MEAKKA;

namespace Booma
{
	public interface ILobbyEntryService
	{
		Task<bool> TryEnterLobbyAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, int characterSlot, int lobbyId, CancellationToken token = default);

		Task<bool> TryEnterLobbyAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, NetworkEntityGuid entity, int lobbyId, CancellationToken token = default);
	}

	public sealed class DefaultLobbyEntryService : ILobbyEntryService
	{
		private ICharacterActorReferenceContainer CharacterActorContainer { get; }

		private ICharacterDataSnapshotFactory CharacterDataFactory { get; }

		//TODO: Don't expose this directly just to resolve an actor reference.
		private ActorSystem GlobalActorSystem { get; }

		private ILog Logger { get; }

		private IEntityActorRef<RootChannelActor> ChannelActor { get; }

		public DefaultLobbyEntryService(ICharacterActorReferenceContainer characterActorContainer, ICharacterDataSnapshotFactory characterDataFactory, ActorSystem globalActorSystem, ILog logger, IEntityActorRef<RootChannelActor> channelActor)
		{
			CharacterActorContainer = characterActorContainer ?? throw new ArgumentNullException(nameof(characterActorContainer));
			CharacterDataFactory = characterDataFactory ?? throw new ArgumentNullException(nameof(characterDataFactory));
			GlobalActorSystem = globalActorSystem ?? throw new ArgumentNullException(nameof(globalActorSystem));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			ChannelActor = channelActor ?? throw new ArgumentNullException(nameof(channelActor));
		}

		public async Task<bool> TryEnterLobbyAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, int characterSlot, int lobbyId, CancellationToken token = default)
		{
			InitialCharacterDataSnapshot dataSnapshot =
				await CharacterDataFactory.Create(new CharacterDataEventPayloadCreationContext(characterSlot));

			return await TryEnterLobbyWithCharacterAsync(context, dataSnapshot, lobbyId, token);
		}

		public async Task<bool> TryEnterLobbyAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, NetworkEntityGuid entity, int lobbyId, CancellationToken token = default)
		{
			InitialCharacterDataSnapshot dataSnapshot =
				await CharacterDataFactory.Create(entity);

			return await TryEnterLobbyWithCharacterAsync(context, dataSnapshot, lobbyId, token);
		}

		private async Task<bool> TryEnterLobbyWithCharacterAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, InitialCharacterDataSnapshot dataSnapshot, int lobbyId, CancellationToken token = default)
		{
			InitializeCharacterDataEventPayload characterDataPayload = new InitializeCharacterDataEventPayload(dataSnapshot.Inventory, BuildLobbyCharacterData(dataSnapshot), 0, dataSnapshot.Bank, dataSnapshot.GuildCard, 0, dataSnapshot.Options);

			await context.MessageService.SendMessageAsync(characterDataPayload, token);

			//NetworkEntityGuid
			var characterActorCreationResponse = await ChannelActor
				.Actor
				.Ask<ResponseModel<string, CharacterActorCreationResponseCode>>(new TryCreateCharacterRequestMessage(dataSnapshot, lobbyId));

			//TODO: Handle lobby re-try logic. Lobby may have been full.
			if (!characterActorCreationResponse.isSuccessful)
			{
				switch (characterActorCreationResponse.ResultCode)
				{
					case CharacterActorCreationResponseCode.GeneralError:
						if(Logger.IsErrorEnabled)
							Logger.Error($"Failed to send create character actor. Reason: {characterActorCreationResponse.ResultCode}");

						await context.ConnectionService.DisconnectAsync();
						return false;
					case CharacterActorCreationResponseCode.UnavailableSpace:
						return false;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}

			//At this point we have an actor path and need to setup the network's interface
			//into Akka. To do this we assign a mutateable actor reference which can be accessed by the message
			//handlers.
			CharacterActorContainer.Reference = await GlobalActorSystem
				.ActorSelection(characterActorCreationResponse.Result)
				.ResolveOne(TimeSpan.FromSeconds(30), token);

			CharacterActorContainer.EntityGuid = dataSnapshot.EntityGuid;

			//Initialize the actor but with no initial state.
			CharacterActorContainer.Reference.TellEntity(new EntityActorStateInitializeMessage<EmptyFactoryContext>(EmptyFactoryContext.Instance));

			CharacterActorContainer.Reference
				.InitializeState(context.MessageService);

			CharacterActorContainer.Reference
				.InitializeState(context.ConnectionService);

			CharacterActorContainer.Reference
				.InitializeState(dataSnapshot.EntityGuid);

			CharacterActorContainer.Reference
				.InitializeState(dataSnapshot);

			//Tell client that pre-join initialization is finished.
			CharacterActorContainer.Reference
				.TellEntity<PreJoinInitializationFinishedMessage>();

			return true;
		}

		private static LobbyCharacterData BuildLobbyCharacterData(InitialCharacterDataSnapshot dataSnapshot)
		{
			return new LobbyCharacterData(dataSnapshot.Stats, 0, 0, dataSnapshot.Progress, 0, String.Empty, 0, dataSnapshot.SpecialCustom, dataSnapshot.GuildCard.SectionId, dataSnapshot.GuildCard.ClassType, dataSnapshot.Version, dataSnapshot.Customization, dataSnapshot.Name);
		}
	}
}
