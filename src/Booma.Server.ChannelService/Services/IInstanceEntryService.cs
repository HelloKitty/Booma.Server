using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;
using Glader.ASP.RPG;
using Glader.ASP.ServiceDiscovery;
using Glader.Essentials;
using GladNet;
using MEAKKA;

namespace Booma
{
	public interface IInstanceEntryService
	{
		/// <summary>
		/// Attempts to join the <see cref="entity"/> into the instance they belong in.
		/// Instance entry service controls what instance they end up in.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="entity"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		Task<bool> TryEnterInstanceAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, NetworkEntityGuid entity, CancellationToken token = default);
	}

	public sealed class DefaultInstanceEntryService : IInstanceEntryService
	{
		private ICharacterActorReferenceContainer CharacterActorContainer { get; }

		//TODO: Don't expose this directly just to resolve an actor reference.
		private ActorSystem GlobalActorSystem { get; }

		private ILog Logger { get; }

		private IEntityActorRef<RootChannelActor> ChannelActor { get; }

		private IServiceResolver<IGroupManagementService> GroupManagementService { get; }

		public DefaultInstanceEntryService(ICharacterActorReferenceContainer characterActorContainer,
			ActorSystem globalActorSystem, 
			ILog logger, 
			IEntityActorRef<RootChannelActor> channelActor,
			IServiceResolver<IGroupManagementService> groupManagementService)
		{
			CharacterActorContainer = characterActorContainer ?? throw new ArgumentNullException(nameof(characterActorContainer));
			GlobalActorSystem = globalActorSystem ?? throw new ArgumentNullException(nameof(globalActorSystem));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			ChannelActor = channelActor ?? throw new ArgumentNullException(nameof(channelActor));
			GroupManagementService = groupManagementService ?? throw new ArgumentNullException(nameof(groupManagementService));
		}

		public async Task<bool> TryEnterInstanceAsync(SessionMessageContext<PSOBBGamePacketPayloadServer> context, NetworkEntityGuid entity, CancellationToken token = default)
		{
			var serviceResult = await GroupManagementService.Create(token);

			if(!serviceResult.isAvailable)
				return false;

			//First we must determine if the entity is in a group
			var groupResponse = await serviceResult.Instance.RetrieveMemberGroupAsync(entity.Identifier, token);

			if (!groupResponse.isSuccessful)
				return false;

			//Knowing the id we can then ask the channel for the actor path to the group.
			var instancePathResponse = await ChannelActor.Actor.RequestAsync(new RetrieveInstanceActorPathRequest()
			{
				InstanceId = groupResponse.Result.Id
			}, token);

			if (!instancePathResponse.isSuccessful)
				return false;

			//At this point we have an actor path and need to setup the network's interface
			//into Akka. To do this we assign a mutateable actor reference which can be accessed by the message
			//handlers.
			IActorRef instanceActorRef;
			try
			{
				instanceActorRef = await GlobalActorSystem
					.ActorSelection(instancePathResponse.Result)
					.ResolveOne(TimeSpan.FromSeconds(30), token);
			}
			catch (ActorNotFoundException e)
			{
				if (Logger.IsErrorEnabled)
					Logger.Error($"Instance: {groupResponse.Result.Id} Actor not found. Error: {e}");

				return false;
			}

			var joinResponse = await instanceActorRef.RequestAsync(new PlayerJoinInstanceRequest()
			{
				PlayerGuid = entity,
			}, token);

			if (!joinResponse.isSuccessful)
				return false;

			//Dispose of the current actor, we're gonna be leaving.
			await CharacterActorContainer.DisposeAsync();

			//At this point we have an actor path and need to setup the network's interface
			//into Akka. To do this we assign a mutateable actor reference which can be accessed by the message
			//handlers.
			CharacterActorContainer.Reference = await GlobalActorSystem
				.ActorSelection(joinResponse.Result)
				.ResolveOne(TimeSpan.FromSeconds(30), token);

			CharacterActorContainer.EntityGuid = entity;

			//Initialize the actor but with no initial state.
			CharacterActorContainer.Reference.TellEntity(new EntityActorStateInitializeMessage<EmptyFactoryContext>(EmptyFactoryContext.Instance));

			CharacterActorContainer.Reference
				.InitializeState(context.MessageService);

			CharacterActorContainer.Reference
				.InitializeState(context.ConnectionService);

			CharacterActorContainer.Reference
				.InitializeState(entity);

			//Tell client that pre-join initialization is finished.
			CharacterActorContainer.Reference
				.TellEntity<PreJoinInitializationFinishedMessage>();

			return true;
		}
	}
}
