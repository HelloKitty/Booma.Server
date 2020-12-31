using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Booma.Proxy;
using Glader.Essentials;
using GladNet;

namespace Booma
{
	/// <summary>
	/// <see cref="IGameInitializable"/> that builds and sends a <see cref="SharedWelcomePayload"/> to the session
	/// on initialization.
	/// </summary>
	public sealed class SharedWelcomeMessageInitializable : IGameInitializable
	{
		private IWelcomeMessageFactory MessageFactory { get; }

		private IMessageSendService<PSOBBGamePacketPayloadServer> SendService { get; }

		public SharedWelcomeMessageInitializable(IWelcomeMessageFactory messageFactory,
			IMessageSendService<PSOBBGamePacketPayloadServer> sendService)
		{
			MessageFactory = messageFactory ?? throw new ArgumentNullException(nameof(messageFactory));
			SendService = sendService ?? throw new ArgumentNullException(nameof(sendService));
		}

		public async Task OnGameInitialized()
		{
			//TODO: If this throws the session could handle, since Engine task will never set an exception and exception won't bubble up.
			await SendService.SendMessageAsync(MessageFactory.Create());
		}
	}
}
