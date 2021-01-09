using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using MEAKKA;

namespace Booma.ChannelActor
{
	/// <summary>
	/// <see cref="RootChannelActor"/> handler for <see cref="TryCreateCharacterRequestMessage"/>.
	/// It will attempt to assign and forward a character creation request to a child <see cref="GameLobbyActor"/>.
	/// </summary>
	[ActorMessageHandler(typeof(RootChannelActor))]
	public sealed class AssignCharacterToLobbyMessageHandler : BaseActorMessageHandler<TryCreateCharacterRequestMessage>
	{
		private ILobbyEntryRepository LobbyRepository { get; }

		public AssignCharacterToLobbyMessageHandler(ILobbyEntryRepository lobbyRepository)
		{
			LobbyRepository = lobbyRepository ?? throw new ArgumentNullException(nameof(lobbyRepository));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, TryCreateCharacterRequestMessage message, CancellationToken token = new CancellationToken())
		{
			//TODO: Better select lobby, lobby can be full! Preferred lobbies exist too.
			//Get the default lobby.
			LobbyEntry entry = await LobbyRepository
				.RetrieveAsync(0, token);

			//Just forward the request for a character to enter
			//to a lobby actor. It can respond with a response.
			context
				.ActorContext
				.ActorSelection(entry.LobbyActorPath)
				.Tell(message, context.Sender);
		}
	}
}
