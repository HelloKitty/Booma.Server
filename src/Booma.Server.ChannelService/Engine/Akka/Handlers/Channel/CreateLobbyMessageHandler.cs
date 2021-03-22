using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Handler for mostly <see cref="RootChannelActor"/> which handles lobby creation requests.
	/// </summary>
	[ActorMessageHandler(typeof(RootChannelActor))]
	public sealed class CreateLobbyMessageHandler : BaseActorMessageHandler<CreateLobbyMessage>
	{
		/// <summary>
		/// The logging service.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The repository for lobby information (for the Channel).
		/// </summary>
		private ILobbyEntryRepository LobbyRepository { get; }

		/// <summary>
		/// The factory that can produce lobby actors.
		/// </summary>
		private IActorFactory<LobbyActor> LobbyActorFactory { get; }

		public CreateLobbyMessageHandler(ILog logger, 
			ILobbyEntryRepository lobbyRepository, 
			IActorFactory<LobbyActor> lobbyActorFactory)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			LobbyRepository = lobbyRepository ?? throw new ArgumentNullException(nameof(lobbyRepository));
			LobbyActorFactory = lobbyActorFactory ?? throw new ArgumentNullException(nameof(lobbyActorFactory));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, CreateLobbyMessage message, CancellationToken token = default)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info($"Channel Lobby {message.LobbyId:D2} Creation Request: {message}");

			IEntityActorRef<LobbyActor> lobbyActor = null;
			try
			{
				lobbyActor = LobbyActorFactory
					.Create(new ActorCreationContext(context.ActorContext));

				lobbyActor.Actor.Tell(new EntityActorStateInitializeMessage<EmptyFactoryContext>(EmptyFactoryContext.Instance));

				if (lobbyActor.Actor.IsNobody())
					throw new InvalidOperationException($"Lobby actor failed to successfully create.");

				if (!await LobbyRepository.TryCreateAsync(new LobbyEntry(message.LobbyId, lobbyActor.Actor.Path.ToString()), token))
					throw new InvalidOperationException($"Failed to store lobby in repository");

				//At this point lobby is registered.
				if(Logger.IsInfoEnabled)
					Logger.Info($"Lobby {message.LobbyId:D2} Created Path: {lobbyActor.Actor.Path}");

				//Set lobby GUID, this is the only way a lobby can know its ID!!
				lobbyActor.Actor.InitializeState(new NetworkEntityGuid(EntityType.Lobby, message.LobbyId));
				lobbyActor.Actor.TellEntity<PostInitializeActorMessage>();
			}
			catch (Exception e)
			{
				//TODO: If we rethrow we kill entire Channel but if we cannot create lobby the channel is worthless??
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to create Lobby {message.LobbyId:D2}. Could not store in Lobby Repository. Reason: {e}");

				//Kill the lobby if we ever made it
				lobbyActor?.Actor?.Tell(PoisonPill.Instance);
			}
		}
	}
}
