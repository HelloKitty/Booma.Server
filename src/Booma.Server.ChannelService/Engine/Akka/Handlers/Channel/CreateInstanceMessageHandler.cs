using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Handler for mostly <see cref="RootChannelActor"/> which handles instance creation requests.
	/// </summary>
	[ActorMessageHandler(typeof(RootChannelActor))]
	public sealed class CreateInstanceMessageHandler : ActorRequestMessageHandler<CreateInstanceMessage, bool>
	{
		/// <summary>
		/// The logging service.
		/// </summary>
		private ILog Logger { get; }

		/// <summary>
		/// The factory that can produce instance actors.
		/// </summary>
		private IActorFactory<InstanceActor> InstanceActorFactory { get; }

		/// <summary>
		/// Repository for instances.
		/// </summary>
		private IInstanceEntryRepository InstanceRepository { get; }

		public CreateInstanceMessageHandler(ILog logger, 
			IActorFactory<InstanceActor> instanceActorFactory, 
			IInstanceEntryRepository instanceRepository)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			InstanceActorFactory = instanceActorFactory ?? throw new ArgumentNullException(nameof(instanceActorFactory));
			InstanceRepository = instanceRepository ?? throw new ArgumentNullException(nameof(instanceRepository));
		}

		/// <inheritdoc />
		protected override async Task<bool> HandleRequestAsync(EntityActorMessageContext context, CreateInstanceMessage message, CancellationToken token = default)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info($"Channel {nameof(InstanceActor)} {message.InstanceId:D2} Creation Request: {message}");

			IEntityActorRef<InstanceActor> instanceActor = null;
			try
			{
				if (await InstanceRepository.ContainsAsync(message.InstanceId, token))
					throw new InvalidOperationException($"Tried to create duplicate {nameof(InstanceActor)} with Id: {message.InstanceId}");

				instanceActor = InstanceActorFactory
					.Create(new ActorCreationContext(context.ActorContext));

				instanceActor.Actor.Tell(new EntityActorStateInitializeMessage<EmptyFactoryContext>(EmptyFactoryContext.Instance));

				if (instanceActor.Actor.IsNobody())
					throw new InvalidOperationException($"{nameof(InstanceActor)} failed to successfully create.");

				if (!await InstanceRepository.TryCreateAsync(new InstanceEntry(message.InstanceId, instanceActor.Actor.Path.ToString()), token))
					throw new InvalidOperationException($"Failed to store {nameof(InstanceActor)} in repository");

				//At this point group is registered.
				if(Logger.IsInfoEnabled)
					Logger.Info($"{nameof(InstanceActor)} {message.InstanceId:D2} Created Path: {instanceActor.Actor.Path}");

				//Set instance GUID, this is the only way a instance can know its ID!!
				instanceActor.Actor.InitializeState(new NetworkEntityGuid(EntityType.Group, message.InstanceId));
				instanceActor.Actor.TellEntity<PostInitializeActorMessage>();
				return true;
			}
			catch (Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Failed to create {nameof(InstanceActor)} {message.InstanceId:D2}. Could not store in Instance Repository. Reason: {e}");

				//Kill the lobby if we ever made it
				instanceActor?.Actor?.Tell(PoisonPill.Instance);
				return false;
			}
		}
	}
}
