using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Common.Logging;
using Glader.ASP.GameConfig;
using GladNet;

namespace Booma
{
	/// <summary>
	/// Message handler for <see cref="CharacterOptionsRequestPayload"/> which returns a <see cref="CharacterOptionsResponsePayload"/>.
	/// </summary>
	public sealed class CharacterOptionsRequestMessageHandler : GameRequestMessageHandler<CharacterOptionsRequestPayload, CharacterOptionsResponsePayload>
	{
		private ILog Logger { get; }

		private IServiceResolver<IKeybindConfigurationService> ConfigServiceResolver { get; }

		public CharacterOptionsRequestMessageHandler(ILog logger, 
			IServiceResolver<IKeybindConfigurationService> configServiceResolver)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			ConfigServiceResolver = configServiceResolver ?? throw new ArgumentNullException(nameof(configServiceResolver));
		}

		/// <inheritdoc />
		protected override async Task<CharacterOptionsResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterOptionsRequestPayload message, CancellationToken token = default)
		{
			if (Logger.IsDebugEnabled)
				Logger.Debug($"Client: {context.Details.ConnectionId} sent Options Request.");

			//We should not try to send defaults if the config service
			//is unavailable, because this will override their config
			//therefore we should just disconnect in that case.
			ServiceResolveResult<IKeybindConfigurationService> resolveResult = await ConfigServiceResolver.Create(token);

			if (!resolveResult.isAvailable)
				return await LogServiceErrorAndDisconnectAsync(context);

			var bindQueryResult = await resolveResult.Instance.RetrieveAccountBindsAsync(token);

			//Success means we can directly send down the stored binary data
			if (bindQueryResult.isSuccessful)
				return new CharacterOptionsResponsePayload(new CharacterOptionsConfiguration(new BindingsConfig(bindQueryResult.Result.KeybindData, Array.Empty<byte>()), 1, new AccountTeamInformation(0, new uint[2], 0, 0, String.Empty, 0)));

			//We have special cases for certain results
			//If not found, that means none exist yet so we should send a default result.
			//Other cases are general failure.
			switch(bindQueryResult.ResultCode)
			{
				case GameConfigQueryResponseCode.ContentNotFound:
					return new CharacterOptionsResponsePayload(new CharacterOptionsConfiguration(BindingsConfig.CreateDefault(), 1, new AccountTeamInformation(0, new uint[2], 0, 0, String.Empty, 0)));
				default:
					return await LogServiceErrorAndDisconnectAsync(context);
			}
		}

		private async Task<CharacterOptionsResponsePayload> LogServiceErrorAndDisconnectAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			if (Logger.IsErrorEnabled)
				Logger.Error($"Service: {nameof(IKeybindConfigurationService)} unavailable or a failure occurred when querying config data.");

			await context.ConnectionService.DisconnectAsync();
			return default;
		}
	}
}
