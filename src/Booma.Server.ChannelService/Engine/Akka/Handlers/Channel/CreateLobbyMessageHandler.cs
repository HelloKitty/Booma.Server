using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using MEAKKA;

namespace Booma
{
	[ActorMessageHandler(typeof(RootChannelActor))]
	public sealed class CreateLobbyMessageHandler : BaseActorMessageHandler<CreateLobbyMessage>
	{
		private ILog Logger { get; }

		public CreateLobbyMessageHandler(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override async Task HandleMessageAsync(EntityActorMessageContext context, CreateLobbyMessage message, CancellationToken token = default)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info($"Channel Lobby Creation Request: {message}");
		}
	}
}
