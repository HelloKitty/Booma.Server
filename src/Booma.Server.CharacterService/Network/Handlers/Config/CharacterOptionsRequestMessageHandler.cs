using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Common.Logging;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Message handler for <see cref="CharacterOptionsRequestPayload"/> which returns a <see cref="CharacterOptionsResponsePayload"/>.
	/// </summary>
	public sealed class CharacterOptionsRequestMessageHandler : GameRequestMessageHandler<CharacterOptionsRequestPayload, CharacterOptionsResponsePayload>
	{
		private ILog Logger { get; }

		public CharacterOptionsRequestMessageHandler(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		protected override async Task<CharacterOptionsResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterOptionsRequestPayload message, CancellationToken token = default)
		{
			if (Logger.IsDebugEnabled)
				Logger.Debug($"Client: {context.Details.ConnectionId} sent Options Request.");

			//TODO: This is basically test code, needs to be properly implemented eventually.
			return new CharacterOptionsResponsePayload(new CharacterOptionsConfiguration(BindingsConfig.CreateDefault(), 1, new AccountTeamInformation(0, new uint[2], 0, 0, String.Empty, 0)));
		}
	}
}
