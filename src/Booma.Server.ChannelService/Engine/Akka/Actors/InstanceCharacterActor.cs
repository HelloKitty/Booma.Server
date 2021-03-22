using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class InstanceCharacterActor : BaseEntityActor<EmptyFactoryContext>
	{
		//We have to use specific type because it contains actor type information.
		public InstanceCharacterActor(ILog logger, DefaultActorMessageHandlerService<InstanceCharacterActor> messageHandlerService)
			: base(logger, messageHandlerService)
		{

		}

		protected override void OnInitialized(EntityActorInitializationSuccessMessage successMessage)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"{nameof(InstanceCharacterActor)} Initialized. Path: {Self.Path}");
		}
	}
}
