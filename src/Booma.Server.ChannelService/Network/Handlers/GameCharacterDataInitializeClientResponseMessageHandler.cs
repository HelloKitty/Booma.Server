using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Glader.ASP.RPG;
using GladNet;

namespace Booma
{
	public sealed class GameCharacterDataInitializeClientResponseMessageHandler : GameMessageHandler<GameCharacterDataInitializeClientResponsePayload>
	{
		private ICharacterActorReferenceContainer ActorContainer { get; }

		private IServiceResolver<IGroupManagementService> GroupManagementService { get; }

		private ILog Logger { get; }

		public GameCharacterDataInitializeClientResponseMessageHandler(ICharacterActorReferenceContainer actorContainer, 
			IServiceResolver<IGroupManagementService> groupManagementService, 
			ILog logger)
		{
			ActorContainer = actorContainer ?? throw new ArgumentNullException(nameof(actorContainer));
			GroupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, GameCharacterDataInitializeClientResponsePayload message, CancellationToken token = new CancellationToken())
		{
			//Dispose of the current actor, player is asking to leave the Game Lobby.
			await ActorContainer.DisposeAsync();

			var groupServiceQueryResult = await GroupManagementService.Create(token);

			if (!groupServiceQueryResult.isAvailable)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to remove Player: {ActorContainer.EntityGuid} from Group. Reason: Group service was not available.");

				await context.ConnectionService.DisconnectAsync();
				return;
			}

			var responseCode = await groupServiceQueryResult.Instance.RemoveMemberAsync(ActorContainer.EntityGuid.Identifier, token);

			//If we were successful the client will eventually ask to a join a lobby so everything will end up alright.
			if (responseCode != GroupMemberManageResponseCode.Success)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Failed to remove Player: {ActorContainer.EntityGuid} from Group. Reason: {responseCode}");

				await context.ConnectionService.DisconnectAsync();
			}
		}
	}
}
