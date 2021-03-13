using System;
using System.Collections.Generic;
using System.Text;
using Common.Logging;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	public sealed class LobbyCharacterActor : BaseEntityActor<EmptyFactoryContext>
	{
		//We have to use specific type because it contains actor type information.
		public LobbyCharacterActor(ILog logger, DefaultActorMessageHandlerService<LobbyCharacterActor> messageHandlerService)
			: base(logger, messageHandlerService)
		{

		}

		protected override void OnInitialized(EntityActorInitializationSuccessMessage successMessage)
		{
			if(Logger.IsInfoEnabled)
				Logger.Info($"{nameof(LobbyCharacterActor)} Initialized. Path: {Self.Path}");
		}
	}
}
