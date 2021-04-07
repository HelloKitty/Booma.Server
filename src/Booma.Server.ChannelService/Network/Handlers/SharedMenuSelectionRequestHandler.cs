using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Booma.UI;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using GladNet;
using MEAKKA;

namespace Booma
{
	public sealed class SharedMenuSelectionRequestHandler : GameMessageHandler<SharedMenuSelectionRequestPayload>
	{
		private IServiceResolver<IGroupManagementService> GroupManagementService { get; }

		private ICharacterActorReferenceContainer ActorContainer { get; }

		private IInstanceEntryService InstanceEntryService { get; }

		public SharedMenuSelectionRequestHandler(IServiceResolver<IGroupManagementService> groupManagementService, 
			ICharacterActorReferenceContainer actorContainer, 
			IInstanceEntryService instanceEntryService)
		{
			GroupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
			ActorContainer = actorContainer ?? throw new ArgumentNullException(nameof(actorContainer));
			InstanceEntryService = instanceEntryService ?? throw new ArgumentNullException(nameof(instanceEntryService));
		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, SharedMenuSelectionRequestPayload message, CancellationToken token = new CancellationToken())
		{
			//Player is trying to join a group
			if (message.Selection.MenuId == (ulong) KnownMenuIdentifier.GAME_TYPE)
			{
				ServiceResolveResult<IGroupManagementService> groupServiceQueryResult = await GroupManagementService.Create(token);

				if(!groupServiceQueryResult.isAvailable)
				{
					await context.ConnectionService.DisconnectAsync();
					return;
				}

				GroupMemberManageResponseCode joinResult = await groupServiceQueryResult
					.Instance.AddMemberAsync((int) message.Selection.ItemId, ActorContainer.EntityGuid.Identifier, token);

				if (joinResult != GroupMemberManageResponseCode.Success)
				{
					await context.MessageService.SendMessageAsync(new SharedCreateMessageBoxEventPayload($"Failed to join game. Reason: {joinResult} Code: {(int)joinResult}."), token);
					return;
				}

				//Otherwise, in the DB they've joined the group so we should join them to the instance.
				if (await InstanceEntryService.TryEnterInstanceAsync(context, ActorContainer.EntityGuid, token))
				{
					ActorContainer.Reference.TellEntity<PostInitializeActorMessage>();
				}
				else
				{
					//Must remove from group in DB if they failed to join the instance.
					await groupServiceQueryResult.Instance.RemoveMemberAsync(ActorContainer.EntityGuid.Identifier, token);
					await context.MessageService.SendMessageAsync(new SharedCreateMessageBoxEventPayload($"Failed to join group. Reason: Failed to join instance."), token);
				}
			}
		}
	}
}
