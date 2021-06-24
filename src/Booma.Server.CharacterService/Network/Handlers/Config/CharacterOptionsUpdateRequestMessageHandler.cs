using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma;
using Common.Logging;
using Glader.ASP.GameConfig;
using Glader.ASP.ServiceDiscovery;
using GladNet;

namespace Booma
{
	//TODO: Need persistence.
	//Client can send this if it disagrees with the options data sent down or wants to update their options.
	/// <summary>
	/// Message handler for <see cref="CharacterOptionsRequestPayload"/> which returns a <see cref="CharacterOptionsResponsePayload"/>.
	/// </summary>
	public sealed class CharacterOptionsUpdateRequestMessageHandler : GameRequestMessageHandler<CharacterOptionsUpdateRequestPayload, CharacterOptionsResponsePayload>
	{
		private IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> ConfigServiceResolver { get; }

		private ILog Logger { get; }

		public CharacterOptionsUpdateRequestMessageHandler(ILog logger, IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> configServiceResolver, bool awaitResponseSend = false) : base(awaitResponseSend)
		{
			ConfigServiceResolver = configServiceResolver ?? throw new ArgumentNullException(nameof(configServiceResolver));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		protected override async Task<CharacterOptionsResponsePayload> HandleRequestAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, CharacterOptionsUpdateRequestPayload message, CancellationToken token = default)
		{
			//We should not try to send defaults if the config service
			//is unavailable, because this will override their config
			//therefore we should just disconnect in that case.
			ServiceResolveResult<IGameConfigurationService<PsobbGameConfigurationType>> resolveResult = await ConfigServiceResolver.Create(token);

			if (!resolveResult.isAvailable)
				return await LogServiceErrorAndDisconnectAsync(context);

			//We don't need to sanity check the length of these buffers because the serializer will ensure they don't exceed the length
			//since they are fixed size

			//Store the binds sent (will be queried at some point by the backend for loading in-game)
			await resolveResult
				.Instance
				.UpdateGameConfigAsync(new GameConfigurationUpdateRequest<PsobbGameConfigurationType>(ConfigurationSourceType.Account, PsobbGameConfigurationType.Key, message.Config.Bindings.KeyConfiguration), token);

			await resolveResult
				.Instance
				.UpdateGameConfigAsync(new GameConfigurationUpdateRequest<PsobbGameConfigurationType>(ConfigurationSourceType.Account, PsobbGameConfigurationType.Joystick, message.Config.Bindings.JoystickConfiguration), token);

			//Echo back, since we persisted this data now
			return new CharacterOptionsResponsePayload(message.Config);
		}

		private async Task<CharacterOptionsResponsePayload> LogServiceErrorAndDisconnectAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context)
		{
			if(context == null) throw new ArgumentNullException(nameof(context));

			if(Logger.IsErrorEnabled)
				Logger.Error($"Service: {nameof(IGameConfigurationService<PsobbGameConfigurationType>)} unavailable or a failure occurred when querying config data.");

			await context.ConnectionService.DisconnectAsync();
			return default;
		}
	}
}
