using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Glader.ASP.GameConfig;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using GladNet;

namespace Booma
{
	public sealed class BlockUpdateConfigRequestMessageHandler : GameMessageHandler<BlockUpdateConfigRequestPayload>
	{
		private ILog Logger { get; }

		private IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> ConfigServiceResolver { get; }

		public BlockUpdateConfigRequestMessageHandler(ILog logger, 
			IServiceResolver<IGameConfigurationService<PsobbGameConfigurationType>> configServiceResolver)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			ConfigServiceResolver = configServiceResolver ?? throw new ArgumentNullException(nameof(configServiceResolver));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockUpdateConfigRequestPayload message, CancellationToken token = default)
		{
			//TODO: We should not hold up packet handling by awaiting here and blocking until the update is finished
			var result = await ConfigServiceResolver.Create(token);

			if (!result.isAvailable)
			{
				if (Logger.IsWarnEnabled)
					Logger.Warn($"Tried to handle Type: {nameof(BlockUpdateConfigRequestPayload)} but failed to resolve service Type: {nameof(IGameConfigurationService<PsobbGameConfigurationType>)}");

				//TODO: We could put this in a re-try queue so that their save isn't lost?
				return;
			}

			await result
				.Instance
				.UpdateGameConfigAsync(new GameConfigurationUpdateRequest<PsobbGameConfigurationType>(ConfigurationSourceType.Character, PsobbGameConfigurationType.ActionBar, message.Data), token);
		}
	}
}
