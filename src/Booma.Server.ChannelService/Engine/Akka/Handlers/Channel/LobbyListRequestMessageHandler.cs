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
	public sealed class LobbyListRequestMessageHandler : BaseActorMessageHandler<LobbyListRequestMessage>
	{
		/// <summary>
		/// The logging service.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The repository for lobby information (for the Channel).
		/// </summary>
		private ILobbyEntryRepository LobbyRepository { get; }

		public LobbyListRequestMessageHandler(ILog logger, ILobbyEntryRepository lobbyRepository)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			LobbyRepository = lobbyRepository ?? throw new ArgumentNullException(nameof(lobbyRepository));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, LobbyListRequestMessage message, CancellationToken token = default)
		{
			//Just query and send the lobby list response.
			LobbyEntry[] entries = await LobbyRepository.RetrieveAllAsync(token);
			message.Answer(context.Sender, new LobbyListResponseMessage(entries));
		}
	}
}
