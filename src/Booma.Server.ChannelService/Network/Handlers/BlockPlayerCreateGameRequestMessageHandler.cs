using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using GladNet;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Default packet handlers for <see cref="BlockPlayerCreateGameRequestPayload"/>
	/// </summary>
	public sealed class BlockPlayerCreateGameRequestMessageHandler : GameMessageHandler<BlockPlayerCreateGameRequestPayload>
	{
		private IServiceResolver<IGroupManagementService> GroupManagementService { get; }

		private ICharacterActorReferenceContainer ActorContainer { get; }

		private IEntityActorRef<RootChannelActor> ChannelActor { get; }

		private IInstanceEntryService InstanceEntryService { get; }

		public BlockPlayerCreateGameRequestMessageHandler(IServiceResolver<IGroupManagementService> groupManagementService, 
			ICharacterActorReferenceContainer actorContainer, 
			IEntityActorRef<RootChannelActor> channelActor, 
			IInstanceEntryService instanceEntryService)
		{
			GroupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
			ActorContainer = actorContainer ?? throw new ArgumentNullException(nameof(actorContainer));
			ChannelActor = channelActor ?? throw new ArgumentNullException(nameof(channelActor));
			InstanceEntryService = instanceEntryService ?? throw new ArgumentNullException(nameof(instanceEntryService));
		}

		public override async Task HandleMessageAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, BlockPlayerCreateGameRequestPayload message, CancellationToken token = default)
		{
			ServiceResolveResult<IGroupManagementService> groupServiceQueryResult = await GroupManagementService.Create(token);

			if (!groupServiceQueryResult.isAvailable)
			{
				//TODO: What do we do if they try to create a group but the service is unavailable??
				await context.ConnectionService.DisconnectAsync();
				return;
			}

			//Try to create the group, but we cannot assume it will be successful.
			//Cheaters may try to send and spoof this packet at anytime
			ResponseModel<RPGGroupData, GroupCreationResponseCode> groupCreationResult = await groupServiceQueryResult
				.Instance.CreateGroupAsync(new RPGGroupCreationRequest(message.GameName, ActorContainer.EntityGuid.Identifier), token);

			//There are many failure cases, but the success case is easy we just remove them from the lobby and add them to a game.
			if (groupCreationResult.isSuccessful)
			{
				//This will create the Group/Instance Actor within the system
				if (await ChannelActor.Actor.RequestAsync(new CreateInstanceMessage(groupCreationResult.Result.Id), token))
				{
					//Now we can join it
					if(await InstanceEntryService.TryEnterInstanceAsync(context, ActorContainer.EntityGuid, token))
					{
						ActorContainer.Reference.TellEntity<PostInitializeActorMessage>();
					}
					else
						await context.MessageService.SendMessageAsync(new SharedCreateMessageBoxEventPayload($"Failed to create game. Reason: Failed to enter created instance."), token);
				}
				else
					await context.MessageService.SendMessageAsync(new SharedCreateMessageBoxEventPayload($"Failed to create game. Reason: Failed create instance."), token);
			}
			else
			{
				//TODO: What packet is sent on failed game join??
				await context.MessageService.SendMessageAsync(new SharedCreateMessageBoxEventPayload($"Failed to create game. Reason: {groupCreationResult.ResultCode}"), token);
			}
		}
	}
}
