using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class GameLobbyActor : BaseEntityActor<EmptyFactoryContext>
	{
		//We have to use specific type because it contains actor type information.
		public GameLobbyActor(ILog logger, DefaultActorMessageHandlerService<GameLobbyActor> messageHandlerService)
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
