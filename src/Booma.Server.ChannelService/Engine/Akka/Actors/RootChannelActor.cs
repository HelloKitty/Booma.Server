using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class RootChannelActor : BaseEntityActor<EmptyFactoryContext>
	{
		private ActorMessageHandlerService<RootChannelActor> MessageHandlerService { get; }

		public RootChannelActor(ILog logger, ActorMessageHandlerService<RootChannelActor> messageHandlerService) 
			: base(logger)
		{
			MessageHandlerService = messageHandlerService ?? throw new ArgumentNullException(nameof(messageHandlerService));
		}

		protected override async Task<bool> OnReceiveMessageAsync(EntityActorMessage message, EntityActorMessageContext context)
		{
			return await MessageHandlerService.HandleMessageAsync(context, message);
		}

		protected override void OnInitialized(EntityActorInitializationSuccessMessage successMessage)
		{
			if (Logger.IsInfoEnabled)
				Logger.Info($"Channel Actor Initialized. Path: {Self.Path}");
		}
	}
}
