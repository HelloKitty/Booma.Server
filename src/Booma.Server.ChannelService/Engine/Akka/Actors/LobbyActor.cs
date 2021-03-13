using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class LobbyActor : BaseEntityActor<EmptyFactoryContext>
	{
		//We have to use specific type because it contains actor type information.
		public LobbyActor(ILog logger, DefaultActorMessageHandlerService<LobbyActor> messageHandlerService)
			: base(logger, messageHandlerService)
		{

		}

		protected override void OnInitialized(EntityActorInitializationSuccessMessage successMessage)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"Channel Actor Initialized. Path: {Self.Path}");
		}
	}
}
