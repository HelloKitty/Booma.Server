using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.Proxy;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// <see cref="BaseGameMessageHandler{TMessageType}"/> for the <see cref="SharedLoginRequest93Payload"/>.
	/// </summary>
	public sealed class LoginRequestMessageHandler : BaseGameMessageHandler<SharedLoginRequest93Payload>
	{
		private ILog Logger { get; }

		public LoginRequestMessageHandler([JetBrains.Annotations.NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, SharedLoginRequest93Payload message, CancellationToken token = default)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info($"Sending Auth Response: Banned for test purposes.");

			await context.MessageService.SendMessageAsync(new SharedLoginResponsePayload(AuthenticationResponseCode.LOGIN_93BB_BANNED), token);
		}
	}
}
