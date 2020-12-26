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
	/// <see cref="GameRequestMessageHandler{TMessageRequestType,TMessageResponseType}"/> for the <see cref="SharedLoginRequest93Payload"/>.
	/// </summary>
	public sealed class LoginRequestMessageHandler : GameRequestMessageHandler<SharedLoginRequest93Payload, SharedLoginResponsePayload>
	{
		private ILog Logger { get; }

		public LoginRequestMessageHandler([NotNull] ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		protected override async Task<SharedLoginResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, 
			SharedLoginRequest93Payload message, CancellationToken token = default)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"Sending Auth Response: Banned for test purposes.");

			return new SharedLoginResponsePayload(AuthenticationResponseCode.LOGIN_93BB_BANNED);
		}
	}
}
