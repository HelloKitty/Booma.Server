using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using Booma;
using Booma.UI;
using Common.Logging;
using Glader.Essentials;
using Glader;
using Glader.ASP.ServiceDiscovery;
using GladNet;
using MEAKKA;

namespace Booma
{
	public sealed class ChannelWelcomeEventListener : LoginResponseSentEventListener
	{
		private ICharacterDataSnapshotFactory CharacterDataFactory { get; }

		private IEntityActorRef<RootChannelActor> ChannelActor { get; }

		private ILog Logger { get; }

		private ICharacterActorReferenceContainer CharacterActorContainer { get; }

		//TODO: Don't expose this directly just to resolve an actor reference.
		private ActorSystem GlobalActorSystem { get; }

		public ChannelWelcomeEventListener(ILoginResponseSentEventSubscribable subscriptionService, 
			IEntityActorRef<RootChannelActor> channelActor, 
			ILog logger, 
			ICharacterDataSnapshotFactory characterDataFactory, 
			ICharacterActorReferenceContainer characterActorContainer, 
			ActorSystem globalActorSystem) 
			: base(subscriptionService)
		{
			ChannelActor = channelActor ?? throw new ArgumentNullException(nameof(channelActor));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			CharacterDataFactory = characterDataFactory ?? throw new ArgumentNullException(nameof(characterDataFactory));
			CharacterActorContainer = characterActorContainer ?? throw new ArgumentNullException(nameof(characterActorContainer));
			GlobalActorSystem = globalActorSystem ?? throw new ArgumentNullException(nameof(globalActorSystem));
		}

		protected override async Task OnEventFiredAsync(object source, LoginResponseSentEventArgs args)
		{
			//Disconnect if not successful
			if (args.ResponseCode != AuthenticationResponseCode.LOGIN_93BB_OK)
			{
				await args.MessageContext.ConnectionService.DisconnectAsync();
				return;
			}

			if (!await SendLobbyListAsync(args.MessageContext)) 
				return;

			InitialCharacterDataSnapshot dataSnapshot = 
				await CharacterDataFactory.Create(new CharacterDataEventPayloadCreationContext(args.CharacterSlot));
			InitializeCharacterDataEventPayload characterDataPayload = new InitializeCharacterDataEventPayload(dataSnapshot.Inventory, BuildLobbyCharacterData(dataSnapshot), 0, dataSnapshot.Bank, dataSnapshot.GuildCard, 0, dataSnapshot.Options);

			await args.MessageContext.MessageService.SendMessageAsync(characterDataPayload);

			//NetworkEntityGuid
			var characterActorCreationResponse = await ChannelActor
				.Actor
				.Ask<ResponseModel<string, CharacterActorCreationResponseCode>>(new TryCreateCharacterRequestMessage(dataSnapshot));

			//TODO: Handle lobby re-try logic. Lobby may have been full.
			if (!characterActorCreationResponse.isSuccessful)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to send create character actor. Reason: {characterActorCreationResponse.ResultCode}");

				await args.MessageContext.ConnectionService.DisconnectAsync();
				return;
			}

			//At this point we have an actor path and need to setup the network's interface
			//into Akka. To do this we assign a mutateable actor reference which can be accessed by the message
			//handlers.
			CharacterActorContainer.Reference = await GlobalActorSystem
				.ActorSelection(characterActorCreationResponse.Result)
				.ResolveOne(TimeSpan.FromSeconds(30), CancellationToken.None);

			CharacterActorContainer.EntityGuid = dataSnapshot.EntityGuid;

			//Initialize the actor but with no initial state.
			CharacterActorContainer.Reference
				.Tell(new EntityActorStateInitializeMessage<EmptyFactoryContext>(EmptyFactoryContext.Instance));

			CharacterActorContainer.Reference
				.InitializeState(args.MessageContext.MessageService);

			CharacterActorContainer.Reference
				.InitializeState(args.MessageContext.ConnectionService);

			CharacterActorContainer.Reference
				.InitializeState(dataSnapshot.EntityGuid);

			CharacterActorContainer.Reference
				.InitializeState(dataSnapshot);

			//Tell client that pre-join initialization is finished.
			CharacterActorContainer.Reference
				.TellEntity<PreJoinInitializationFinishedMessage>();

			/*await args.MessageContext.MessageService.SendMessageAsync(new BlockLobbyJoinEventPayload(0, 0, 0, 1, 0, new CharacterJoinData[1]
			{
				new CharacterJoinData(new PlayerInformationHeader(1, 0, "Glader"), new CharacterInventoryData(0, 0, 0, 0, Enumerable.Repeat(new InventoryItem(), 30).ToArray()), ChannelWelcomeEventListener.CreateDefaultCharacterData()),
			}), CancellationToken.None);*/

			//await args.MessageContext.MessageService.SendMessageAsync(new BlockCharacterDataInitializationServerRequestPayload());
		}

		private static LobbyCharacterData BuildLobbyCharacterData(InitialCharacterDataSnapshot dataSnapshot)
		{
			return new LobbyCharacterData(dataSnapshot.Stats, 0, 0, dataSnapshot.Progress, 0, String.Empty, 0, dataSnapshot.SpecialCustom, dataSnapshot.GuildCard.SectionId, dataSnapshot.GuildCard.ClassType, dataSnapshot.Version, dataSnapshot.Customization, dataSnapshot.Name);
		}

		private async Task<bool> SendLobbyListAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context)
		{
			try
			{
				var lobbyData = (await ChannelActor.Actor
						.Ask<LobbyListResponseMessage>(new LobbyListRequestMessage(), TimeSpan.FromSeconds(15)))
					.Select(l => new LobbyMenuEntry((uint) KnownMenuIdentifier.LOBBY, (uint) l.LobbyId))
					.ToArray();

				if (Logger.IsDebugEnabled)
					Logger.Debug($"Sending Lobby Data Count: {lobbyData.Length}");

				await context.MessageService.SendMessageAsync(new LobbyListEventPayload(lobbyData));
			}
			catch (Exception e)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Failed to send LobbyList. Reason: {e}");

				await context.ConnectionService.DisconnectAsync();
				return false;
			}

			return true;
		}
	}
}
